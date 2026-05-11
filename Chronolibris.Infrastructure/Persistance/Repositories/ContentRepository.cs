using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Chronolibris.Infrastructure.Persistence.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly ApplicationDbContext _context;

        public ContentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Content?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Contents
                .Include(c => c.Country)
                .Include(c => c.Language)
                .Include(c => c.ContentType)
                //.Include(c => c.ParentContent)
                .Include(c => c.Themes)
                .Include(c => c.Participations)
                    .ThenInclude(p => p.Person)
                .Include(c => c.Tags)
                    .ThenInclude(t => t.TagType)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<List<TagDetails>> GetTagsAsync(long contentId, CancellationToken ct)
        {
            var content = await _context.Contents
                .Where(c => c.Id == contentId)
                .SelectMany(c => c.Tags)
                .Select(t => new TagDetails
                {
                    Id = t.Id,
                    Name = t.Name,
                    TagTypeId = t.TagTypeId,
                    TagTypeName = t.TagType.Name
                })
                .ToListAsync(ct);

            return content;
        }

        public async Task<bool> AddTagAsync(long contentId, long tagId, CancellationToken ct)
        {
            var content = await _context.Contents
                .Include(c => c.Tags)
                .FirstOrDefaultAsync(c => c.Id == contentId, ct);

            if (content == null)
                return false;

            var tag = await _context.Tags.FindAsync(new object[] { tagId }, ct);
            if (tag == null)
                return false;

            if (!content.Tags.Any(t => t.Id == tagId))
            {
                content.Tags.Add(tag);
                await _context.SaveChangesAsync(ct);
            }

            return true;
        }

        public async Task<bool> RemoveTagAsync(long contentId, long tagId, CancellationToken ct)
        {
            var content = await _context.Contents
                .Include(c => c.Tags)
                .FirstOrDefaultAsync(c => c.Id == contentId, ct);

            if (content == null)
                return false;

            var tag = content.Tags.FirstOrDefault(t => t.Id == tagId);
            if (tag != null)
            {
                content.Tags.Remove(tag);
                await _context.SaveChangesAsync(ct);
                return true;
            }

            return false;
        }

        public async Task<List<BookDto>> GetBooksDtoByContentIdAsync(long contentId, CancellationToken cancellationToken)
        {
            return await _context.Books
                .AsNoTracking()
                .Where(b => b.BookContents.Any(bc => bc.ContentId == contentId))
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    CountryId = b.CountryId,
                    CountryName = b.Country.Name,
                    LanguageId = b.LanguageId,
                    LanguageName = b.Language.Name,
                    Year = b.Year,
                    ISBN = b.ISBN,
                    CoverPath = b.CoverPath,
                    IsAvailable = b.IsAvailable,
                    IsReviewable = b.IsReviewable,
                    PublisherId = b.PublisherId,
                    PublisherName = b.Publisher!= null ? b.Publisher.Name : "",
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,

                    Authors = b.BookContents.SelectMany(bc => bc.Content.Participations)
                    .Where(cp => cp.PersonRoleId == 1)
                        .Select(bp => bp.Person.Name)
                        .Distinct()
                        .ToList(),

                    Themes = b.BookContents
                        .SelectMany(bc => bc.Content.Themes)
                        .Select(t => new ThemeDto
                        {
                            Id = t.Id,
                            Name = t.Name
                        })
                        .Distinct()
                        .ToList()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<ContentDto?> GetDtoByIdAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Contents
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new ContentDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    CountryId = c.CountryId,
                    CountryName = c.Country.Name,
                    ContentTypeId = c.ContentTypeId,
                    ContentType = c.ContentType.Name,
                    LanguageId = c.LanguageId,
                    LanguageName = c.Language.Name,
                    YearFrom = c.YearFrom,
                    YearTo = c.YearTo,
                    CreatedAt = c.CreatedAt,

                    Authors = c.Participations.Where(p => p.PersonRoleId == 1)
                        .Select(p => p.Person.Name)
                        .ToList(),

                    Themes = c.Themes.Select(t => new ThemeDto
                    {
                        Id = t.Id,
                        Name = t.Name
                    }).ToList(),

                    BooksCount = c.BookContents.Count(),

                    Participants = c.Participations
                        .GroupBy(p => p.PersonRoleId)
                        .Select(g => new PersonRoleFilter
                        {
                            RoleId = g.Key,
                            PersonIds = g.Select(p => p.PersonId).ToList(),
                            PersonNames = g.Select(p => p.Person.Name).ToList()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedResult<ContentDto>> GetWithFilterAsync(
            ContentFilterRequest filter, CancellationToken cancellationToken = default)
        {
            var query = _context.Contents
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            {
                query = query.Where(t => EF.Functions.ILike(t.Title, $"%{filter.SearchQuery}%"));
            }


            if (filter.LastId != null)
            {

                query = query.Where(c => c.Id > filter.LastId);

            }

            query = query.OrderBy(c => c.Id);

            var projectedQuery = query.Select(c => new ContentDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                CountryId = c.CountryId,
                CountryName = c.Country.Name,
                ContentTypeId = c.ContentTypeId,
                ContentType = c.ContentType.Name,
                LanguageId = c.LanguageId,
                LanguageName = c.Language.Name,
                YearFrom = c.YearFrom,
                YearTo = c.YearTo,
                CreatedAt = c.CreatedAt,
                Authors = c.Participations.Where(p => p.PersonRoleId==1).Select(p => p.Person.Name)
                .ToList(),

                Themes = c.Themes.Select(t => new ThemeDto
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList(),

                BooksCount = c.BookContents.Count(),
               

                //BooksCount = _context.Books.Count(b => b.BookContents.Any(bc=> bc.ContentId == c.Id)),
            });

            var items = await projectedQuery.Take(filter.Limit + 1).ToListAsync(cancellationToken);

            var hasMore = items.Count > filter.Limit;
            if (hasMore)
            {
                items.RemoveAt(items.Count - 1);
            }

            return new PagedResult<ContentDto>
            {
                HasNext = hasMore,
                Items = items,
                LastId = items.LastOrDefault()?.Id ?? filter.LastId,
                Limit = filter.Limit
            };
        }

        public async Task AddAsync(Content content, CancellationToken cancellationToken = default)
        {
            await _context.Contents.AddAsync(content, cancellationToken);
        }

        //public void Update(Content content)
        //{
        //    _context.Contents.Update(content);
        //}

        public void Delete(Content content)
        {
            _context.Contents.Remove(content);
        }

        public async Task LinkContentToBookAsync(long contentId, long bookId, CancellationToken cancellationToken = default)
        {
            try
            {
                var bookContent = new BookContent
                {
                    ContentId = contentId,
                    BookId = bookId,
                    //Order = order
                };

                await _context.Set<BookContent>().AddAsync(bookContent, cancellationToken);
                await _context.SaveChangesAsync();
            } catch (DbUpdateException ex) when(ex.InnerException is PostgresException pgEx)
            {
                if (pgEx.SqlState == "23503")
                {
                    throw new ChronolibrisException("Книга или подборка не найдена", ErrorType.NotFound);
                }
                if (pgEx.SqlState == "23505") //если уже привязано, ничего не вывожу
                    return;
                throw;
            }
        }

        public async Task UnlinkContentFromBookAsync(long contentId, long bookId, CancellationToken cancellationToken = default)
        {
            var bookContent = await _context.Set<BookContent>()
                .FirstOrDefaultAsync(bc => bc.ContentId == contentId && bc.BookId == bookId, cancellationToken);

            if (bookContent != null)
            {
                _context.Set<BookContent>().Remove(bookContent);
            }
            await _context.SaveChangesAsync();
        }

        public void SyncThemes(Content content, List<long> newThemeIds)
        {
            var toRemove = content.Themes
                .Where(t => !newThemeIds.Contains(t.Id))
                .ToList();

            foreach (var theme in toRemove)
                content.Themes.Remove(theme);

            var currentIds = content.Themes.Select(t => t.Id).ToHashSet();
            foreach (var themeId in newThemeIds.Where(id => !currentIds.Contains(id)))
            {
                var trackedTheme = _context.Themes.Local.FirstOrDefault(t => t.Id == themeId);

                if (trackedTheme != null)
                {
                    //Если тема уже загружена, нужно использовать ее
                    content.Themes.Add(trackedTheme);
                }
                else
                {
                    // Если её нет — нужно создать заглушку и пометить как Unchanged
                    //Без проверки может выдать исключение (две сущности с одинаковым идентификатором)
                    var themeStub = new Theme { Id = themeId };
                    _context.Entry(themeStub).State = EntityState.Unchanged;
                    content.Themes.Add(themeStub);
                }
                //У меня нет промежуточной сущности, но если бы была,
                //то можно было бы использовать ее по аналогии с персоналиями - тогда
                //такой проблемы бы не возникло
            }
        }

        public void SyncParticipations(Content content, List<PersonRoleFilter> personFilters)
        {
            var desiredPairs = personFilters
                .SelectMany(f => f.PersonIds.Select(pid => (PersonId: pid, RoleId: f.RoleId)))
                .ToHashSet();

            var toRemove = content.Participations
                .Where(p => !desiredPairs.Contains((p.PersonId, p.PersonRoleId)))
                .ToList();

            foreach (var participation in toRemove)
                content.Participations.Remove(participation);

            var currentPairs = content.Participations
                .Select(p => (p.PersonId, p.PersonRoleId))
                .ToHashSet();

            foreach (var pair in desiredPairs.Where(dp => !currentPairs.Contains(dp)))
            {
                content.Participations.Add(new ContentParticipation
                {
                    PersonId = pair.PersonId,
                    PersonRoleId = pair.RoleId
                });
            }
        }
    }
}