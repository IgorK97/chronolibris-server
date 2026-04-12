using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.Persistence.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Content> _set;

        public ContentRepository(ApplicationDbContext context)
        {
            _context = context;
            _set = context.Set<Content>();
        }

        public async Task<Content?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _set
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

        public async Task<List<TagDetails>> SearchTagsAsync(
            string searchTerm,
            long? tagTypeId,
            int limit,
            CancellationToken ct)
        {
            IQueryable<Tag> query = _context.Tags.AsNoTracking()
                .Include(t => t.TagType);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t => EF.Functions.Like(t.Name, $"%{searchTerm}%"));
            }

            if (tagTypeId.HasValue)
            {
                query = query.Where(t => t.TagTypeId == tagTypeId.Value);
            }

            var tags = await query
                .OrderBy(t => t.Name)
                .Take(limit)
                .Select(t => new TagDetails
                {
                    Id = t.Id,
                    Name = t.Name,
                    TagTypeId = t.TagTypeId,
                    TagTypeName = t.TagType.Name
                })
                .ToListAsync(ct);

            return tags;
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


        public async Task<IReadOnlyList<Content>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _set.ToListAsync(cancellationToken);
        }
        //Simplify this!!!

        public async Task<(List<Content> Items, int TotalCount, string? NextCursor, string? PrevCursor)> GetWithFilterAsync(
            ContentFilterRequest filter, CancellationToken cancellationToken = default)
        {
            var query = _set
                .Include(c => c.Country)
                .Include(c => c.Language)
                .Include(c => c.ContentType)
                .Include(c => c.Themes)
                .Include(c => c.Participations)
                    .ThenInclude(p => p.Person)
                .AsQueryable();

            // Поиск по названию
            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            {
                query = query.Where(c => c.Title.Contains(filter.SearchQuery));
            }

            // Фильтр по автору
            if (!string.IsNullOrWhiteSpace(filter.AuthorName))
            {
                query = query.Where(c => c.Participations.Any(p =>
                    p.Person.Name.Contains(filter.AuthorName)));
            }
            if (filter.PersonFilters != null)
            {
                foreach (var filt in filter.PersonFilters)
                {
                    var roleId = filt.RoleId;
                    var personIds = filt.PersonIds;

                    if (personIds != null && personIds.Count>0)
                    {
                        query = query.Where(c => c.Participations.Any(p => p.PersonRoleId == roleId && personIds.Contains(p.PersonId)));
                    }
                }
            }


            // Получаем общее количество
            var totalCount = await query.CountAsync(cancellationToken);

            // Курсорная пагинация
            if (!string.IsNullOrWhiteSpace(filter.Cursor))
            {
                if (long.TryParse(filter.Cursor, out var cursorId))
                {
                    query = query.Where(c => c.Id > cursorId);
                }
            }

            query = query.OrderBy(c => c.Id);

            var items = await query.Take(filter.Limit + 1).ToListAsync(cancellationToken);

            var hasMore = items.Count > filter.Limit;
            if (hasMore)
            {
                items.RemoveAt(items.Count - 1);
            }

            var nextCursor = hasMore ? items.Last().Id.ToString() : null;
            var prevCursor = filter.Cursor != null ? filter.Cursor : null;

            return (items, totalCount, nextCursor, prevCursor);
        }

        public async Task AddAsync(Content content, CancellationToken cancellationToken = default)
        {
            await _set.AddAsync(content, cancellationToken);
        }

        public void Update(Content content)
        {
            _set.Update(content);
        }

        public void Delete(Content content)
        {
            _set.Remove(content);
        }

        public async Task<int> GetBooksCountAsync(long contentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BookContent>()
                .CountAsync(bc => bc.ContentId == contentId, cancellationToken);
        }

        public async Task<List<Book>> GetBooksByContentIdAsync(long contentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BookContent>()
                .Include(bc => bc.Book)
                    .ThenInclude(b => b.Country)
                .Include(bc => bc.Book)
                    .ThenInclude(b => b.Language)
                .Include(bc => bc.Book)
                    .ThenInclude(b => b.Publisher)
                //.Include(bc => bc.Book)
                    //.ThenInclude(b => b.Series)
                .Include(bc => bc.Book)
                    .ThenInclude(b => b.Participations)
                        .ThenInclude(p => p.Person)

                .Where(bc => bc.ContentId == contentId)
                //.OrderBy(bc => bc.Order)
                .Select(bc => bc.Book)
                .ToListAsync(cancellationToken);
        }

        public async Task LinkContentToBookAsync(long contentId, long bookId, CancellationToken cancellationToken = default)
        {
            var bookContent = new BookContent
            {
                ContentId = contentId,
                BookId = bookId,
                //Order = order
            };

            await _context.Set<BookContent>().AddAsync(bookContent, cancellationToken);
        }

        public async Task UnlinkContentFromBookAsync(long contentId, long bookId, CancellationToken cancellationToken = default)
        {
            var bookContent = await _context.Set<BookContent>()
                .FirstOrDefaultAsync(bc => bc.ContentId == contentId && bc.BookId == bookId, cancellationToken);

            if (bookContent != null)
            {
                _context.Set<BookContent>().Remove(bookContent);
            }
        }

        public async Task<List<string>> GetAuthorNamesByContentIdAsync(long contentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ContentParticipation>()
                .Include(cp => cp.Person)
                .Where(cp => cp.ContentId == contentId)
                .Select(cp => cp.Person.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Theme>> GetThemesByContentIdAsync(long contentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Content>()
                .Where(c => c.Id == contentId)
                .SelectMany(c => c.Themes)
                .ToListAsync(cancellationToken);
        }

        public async Task SyncThemesAsync(long contentId, List<long> newThemeIds, CancellationToken cancellationToken = default)
        {
            var content = await _context.Contents.Include(c=>c.Themes)
                .FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken) ??
                throw new KeyNotFoundException($"Not found");

            var currentIds = content.Themes.Select(t => t.Id).ToHashSet();
            var desiredIds = newThemeIds.ToHashSet();

            var toRemove = content.Themes.Where(t => !desiredIds.Contains(t.Id)).ToList();
            foreach (var theme in toRemove)
                content.Themes.Remove(theme);

            var toAddIds = desiredIds.Except(currentIds).ToList();
            if (toAddIds.Count > 0)
            {
                var themesToAdd = await _context.Themes.Where(t=>toAddIds.Contains(t.Id))
                    .ToListAsync(cancellationToken);
                foreach (var theme in themesToAdd)
                    content.Themes.Add(theme);
            }
        }

        public async Task SyncPersonsAsync(long contentId, List<PersonRoleFilter> newPersons, CancellationToken cancellationToken = default)
        {
           var content = await _context.Contents.Include(c=>c.Participations).
                FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken) ??
                throw new KeyNotFoundException($"Not found");

            var currentPairs = content.Participations.Select(p => (p.PersonId,p.PersonRoleId))
                .ToHashSet();
            var desiredPairs = newPersons.SelectMany(pr => pr.PersonIds.Select(pid => (PersonId: pid, PersonRoleId: pr.RoleId)))
                .ToHashSet();

            var pairsToAdd = desiredPairs.Except(currentPairs).ToList();
            foreach (var pair in pairsToAdd)
            {
                var participation = new ContentParticipation
                {
                    PersonRoleId = pair.PersonRoleId,
                    PersonId = pair.PersonId,
                };
                content.Participations.Add(participation);
            }

            var pairsToRemove = currentPairs.Except(desiredPairs).ToList();
            var participationsToRemove = content.Participations.Where(p =>
                pairsToRemove.Contains((p.PersonId, p.PersonRoleId))).ToList();

            foreach (var participation in participationsToRemove)
                content.Participations.Remove(participation);




        }

        public async Task SyncTagsAsync(long contentId, List<long> TagIds, CancellationToken cancellationToken)
        {
            var content = await _context.Contents.Include(c => c.Tags)
               .FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken) ??
               throw new KeyNotFoundException($"Not found");

            var currentIds = content.Tags.Select(t => t.Id).ToHashSet();
            var desiredIds = TagIds.ToHashSet();

            var toRemove = content.Tags.Where(t => !desiredIds.Contains(t.Id)).ToList();
            foreach (var tag in toRemove)
                content.Tags.Remove(tag);

            var toAddIds = desiredIds.Except(currentIds).ToList();
            if (toAddIds.Count > 0)
            {
                var tagsToAdd = await _context.Tags.Where(t => toAddIds.Contains(t.Id))
                    .ToListAsync(cancellationToken);
                foreach (var tag in tagsToAdd)
                    content.Tags.Add(tag);
            }
        }
    }
}