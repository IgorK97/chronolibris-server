using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using Chronolibris.Domain.Utils;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.Persistance.Repositories
{

    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Content>> GetContentsWithDetailsByBookIdAsync(long bookId, CancellationToken ct)
        {
            return await _context.Contents
                .AsNoTracking()
                .Where(c => c.Id == bookId)
                .Include(c => c.Country)
                .Include(c => c.ContentType)
                .Include(c => c.Language)
                .Include(c => c.Themes)
                .Include(c => c.Participations)
                    .ThenInclude(p => p.Person)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync(ct);
        }
        public async Task<BookDetails?> GetBookWithRelationsAsync(long bookId, long userId, bool mode, CancellationToken token)
        {
            var raw = await _context.Books
                .Where(b => b.Id == bookId).AsSplitQuery()
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    b.Year,
                    b.ISBN,
                    b.IsAvailable,
                    b.IsReviewable,
                    b.CoverPath,
                    b.Bbk,
                    b.Udk,
                    b.Source,
                    CountryId = b.CountryId,
                    LanguageId = b.LanguageId,
                    CountryName = b.Country.Name,
                    LanguageName = b.Language.Name,
                    PublisherId = b.PublisherId,
                    PublisherName = b.Publisher != null ? b.Publisher.Name : null,
                    Stats = b.IsReviewable ? new
                    {
                        AverageRating = b.Reviews
                                        .Where(r => !r.IsDeleted)
                                        .Average(r => (decimal?)r.Score) ??0m,
                        RatingsCount = b.Reviews.Count(r => !r.IsDeleted),
                        ReviewsCount = b.Reviews.Count(r => r.ReviewText != null && !r.IsDeleted),
                        CommentsCount = b.Comments.Count(),
                        UserRating = b.Reviews.Where(r => r.UserId == userId && !r.IsDeleted)
                                        .Select(r => (decimal?)r.Score).FirstOrDefault()
                    } : null,
                    IsFavorite = b.Shelves.Any(s => s.UserId == userId && s.ShelfTypeId == ShelfTypes.FAVORITES_CODE),
                    IsRead = b.Shelves.Any(s => s.UserId == userId && s.ShelfTypeId == ShelfTypes.READ_CODE),
                    DirectParticipations = b.Participations.Select(p => new
                    {
                        p.PersonRoleId,
                        RoleName = p.PersonRole.Name,
                        p.PersonId,
                        PersonName = p.Person.Name
                    }),
                    ContentParticipations = b.BookContents
                        .SelectMany(bc => bc.Content.Participations)
                        .Select(p => new
                        {
                            p.PersonRoleId,
                            RoleName = p.PersonRole.Name,
                            p.PersonId,
                            PersonName = p.Person.Name
                        }),
                    Themes = b.BookContents
                        .SelectMany(bc => bc.Content.Themes)
                        .Select(t => new { t.Id, t.Name }),
                    Tags = b.BookContents.SelectMany(bc => bc.Content.Tags).Distinct()
                    .Select(t=> new TagShortDetails(t.Id, t.Name, t.TagTypeId, t.TagType.Name))
                })
                .FirstOrDefaultAsync(token);

            if (raw == null) return null;
            if (!raw.IsAvailable && !mode) return null;

            var allParticipations = raw.DirectParticipations
                .Concat(raw.ContentParticipations);

            return new BookDetails
            {
                Id = raw.Id,
                Title = raw.Title,
                Description = raw.Description,
                Year = raw.Year,
                ISBN = raw.ISBN,
                Bbk = raw.Bbk,
                Udk = raw.Udk,
                Source = raw.Source,
                IsAvailable = raw.IsAvailable,
                IsReviewable = raw.IsReviewable,
                CoverUri = raw.CoverPath,
                Country = new()
                {
                    Name = raw.CountryName,
                    Id = raw.CountryId
                },
                Language = new()
                {
                    Name = raw.LanguageName,
                    Id = raw.LanguageId
                },
                Publisher = raw.PublisherName != null
                    ? new PublisherDetails { Id = (long)raw.PublisherId!, Name = raw.PublisherName }
                    : null,
                AverageRating = raw.Stats?.AverageRating ?? 0M,
                RatingsCount = raw.Stats?.RatingsCount ?? 0,
                ReviewsCount = raw.Stats?.ReviewsCount ?? 0,
                CommentsCount = raw.Stats?.CommentsCount ?? 0,
                UserRating = raw.Stats?.UserRating ?? 0M,
                IsFavorite = raw.IsFavorite,
                IsRead = raw.IsRead,
                Participants = allParticipations
                    .GroupBy(p => p.PersonRoleId)
                    .Select(g => new BookPersonGroupDetails
                    {
                        Role = g.Key,
                        Persons = g.GroupBy(p => p.PersonId)
                                   .Select(pg => new PersonDetails
                                   {
                                       Id = pg.Key,
                                       FullName = pg.First().PersonName
                                   }).ToList()
                    }).ToList(),
                Themes = raw.Themes
                    .DistinctBy(t => t.Id)
                    .Select(t => new ThemeDetails { Id = t.Id, Name = t.Name })
                    .ToList(),
                Tags = raw.Tags
            };
        }

        public override async Task<Book?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Books
                .Include(b => b.Country)
                .Include(b => b.Language)
                .Include(b => b.Publisher)
                //.Include(b => b.Series)
                .Include(b => b.Participations)
                    .ThenInclude(p => p.Person)
                    .Include(b => b.BookContents)
                    .ThenInclude(bc => bc.Content)
                        .ThenInclude(c => c.Themes)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public override async Task<List<Book>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Books.ToListAsync(cancellationToken);
        }

        //public async Task<(List<Book> Items, int TotalCount, string? NextCursor, string? PrevCursor)> GetWithFilterAsync(
        //    BookFilterRequest filter, CancellationToken cancellationToken = default)
        //{
        //    var query = _set
        //        .Include(b => b.Country)
        //        .Include(b => b.Language)
        //        .Include(b => b.Publisher)
        //        //.Include(b => b.Series)
        //        .Include(b => b.Participations)
        //            .ThenInclude(p => p.Person)
        //        .Include(b=>b.BookContents)
        //            .ThenInclude(bc=>bc.Content)
        //                .ThenInclude(c=>c.Themes)
        //        .AsQueryable();

        //    // Поиск по названию
        //    if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
        //    {
        //        query = query.Where(b => b.Title.Contains(filter.SearchQuery));
        //    }

        //    // Фильтр по автору
        //    if (!string.IsNullOrWhiteSpace(filter.AuthorName))
        //    {
        //        query = query.Where(b => b.Participations.Any(p =>
        //            p.Person.Name.Contains(filter.AuthorName)));
        //    }

        //    if (filter.IncludeThemeIds != null && filter.IncludeThemeIds.Any())
        //    {
        //        query = query.Where(b => b.BookContents.Any(bc =>
        //            bc.Content.Themes.Any(t => filter.IncludeThemeIds.Contains(t.Id))));
        //    }

        //    if (filter.ExcludeThemeIds != null && filter.ExcludeThemeIds.Any())
        //    {
        //        query = query.Where(b => !b.BookContents.Any(bc =>
        //            bc.Content.Themes.Any(t => filter.ExcludeThemeIds.Contains(t.Id))));
        //    }

        //    if (filter.PublisherId.HasValue)
        //    {
        //        query = query.Where(b => b.PublisherId == filter.PublisherId.Value);
        //    }

        //    if (filter.LanguageId.HasValue)
        //    {
        //        query = query.Where(b => b.LanguageId == filter.LanguageId.Value);
        //    }


        //    if (filter.YearFrom.HasValue)
        //    {
        //        query = query.Where(b => b.Year >= filter.YearFrom.Value);
        //    }
        //    if (filter.YearTo.HasValue)
        //    {
        //        query = query.Where(b => b.Year <= filter.YearTo.Value);
        //    }

        //    if (filter.IsAvailable.HasValue)
        //    {
        //        query = query.Where(b => b.IsAvailable == filter.IsAvailable.Value);
        //    }


        //    var totalCount = await query.CountAsync(cancellationToken);

        //    if (!string.IsNullOrWhiteSpace(filter.Cursor))
        //    {
        //        if (long.TryParse(filter.Cursor, out var cursorId))
        //        {
        //            query = query.Where(b => b.Id > cursorId);
        //        }
        //    }

        //    query = query.OrderBy(b => b.Id);

        //    var items = await query.Take(filter.Limit + 1).ToListAsync(cancellationToken);

        //    var hasMore = items.Count > filter.Limit;
        //    if (hasMore)
        //    {
        //        items.RemoveAt(items.Count - 1);
        //    }

        //    var nextCursor = hasMore ? items.Last().Id.ToString() : null;
        //    var prevCursor = filter.Cursor != null ? filter.Cursor : null;

        //    return (items, totalCount, nextCursor, prevCursor);
        //}

        public async Task<long> CreateAsync(Book book, List<PersonRoleFilter>? personFilter, CancellationToken cancellationToken = default)
        {
            if(personFilter != null)
            {
                book.Participations = new List<BookParticipation>();
                foreach(var roleFilter in personFilter)
                {
                    if (roleFilter.PersonIds == null) continue;
                    foreach(var personId in roleFilter.PersonIds)
                    {
                        book.Participations.Add(new BookParticipation
                        {
                            BookId = book.Id,
                            PersonId = personId,
                            PersonRoleId = roleFilter.RoleId
                        });
                    }
                }
            }


            await _set.AddAsync(book, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return book.Id;
        }

        public void Update(Book book)
        {
            _set.Update(book);
        }

        public void Delete(Book book)
        {
            _set.Remove(book);
        }

        //public async Task<List<string>> GetAuthorNamesByBookIdAsync(long bookId, CancellationToken cancellationToken = default)
        //{
        //    return await _context.Set<BookParticipation>()
        //        .Include(bp => bp.Person)
        //        .Where(bp => bp.BookId == bookId)
        //        .Select(bp => bp.Person.Name)
        //        .ToListAsync(cancellationToken);
        //}

        //public async Task<List<Theme>> GetThemesByBookIdAsync(long bookId, CancellationToken cancellationToken = default)
        //{
        //    return await _context.Set<BookContent>()
        //        .Include(bc => bc.Content)
        //            .ThenInclude(c => c.Themes)
        //        .Where(bc => bc.BookId == bookId)
        //        .SelectMany(bc => bc.Content.Themes)
        //        .Distinct()
        //        .ToListAsync(cancellationToken);
        //}

        //public async Task<bool> IsLinkedToContentAsync(long bookId, long contentId, CancellationToken cancellationToken = default)
        //{
        //    return await _context.Set<BookContent>()
        //        .AnyAsync(bc => bc.BookId == bookId && bc.ContentId == contentId, cancellationToken);
        //}

        //public async Task<int> GetContentsCountAsync(long bookId, CancellationToken cancellationToken = default)
        //{
        //    return await _context.Set<BookContent>()
        //        .CountAsync(bc => bc.BookId == bookId, cancellationToken);
        //}

        //public async Task<List<Content>> GetContentsByBookIdAsync(long bookId, CancellationToken cancellationToken = default)
        //{
        //    return await _context.Set<BookContent>()
        //        .Include(bc => bc.Content)
        //            .ThenInclude(c => c.Country)
        //        .Include(bc => bc.Content)
        //            .ThenInclude(c => c.Language)
        //        .Include(bc => bc.Content)
        //            .ThenInclude(c => c.ContentType)
        //        .Include(bc => bc.Content)
        //            .ThenInclude(c => c.Themes)
        //        .Include(bc => bc.Content)
        //            .ThenInclude(c => c.Participations)
        //                .ThenInclude(cp => cp.Person)
        //        .Where(bc => bc.BookId == bookId)
        //        //.OrderBy(bc => bc.Order)
        //        .Select(bc => bc.Content)
        //        .ToListAsync(cancellationToken);
        //}

        //public async Task LinkContentToBookAsync(long bookId, long contentId, CancellationToken cancellationToken = default)
        //{
        //    var bookContent = new BookContent
        //    {
        //        BookId = bookId,
        //        ContentId = contentId,
        //        //Order = order
        //    };

        //    await _context.Set<BookContent>().AddAsync(bookContent, cancellationToken);
        //}

        //public async Task UnlinkContentFromBookAsync(long bookId, long contentId, CancellationToken cancellationToken = default)
        //{
        //    var bookContent = await _context.Set<BookContent>()
        //        .FirstOrDefaultAsync(bc => bc.BookId == bookId && bc.ContentId == contentId, cancellationToken);

        //    if (bookContent != null)
        //    {
        //        _context.Set<BookContent>().Remove(bookContent);
        //    }
        //}

        public void SyncParticipations(Book book , List<PersonRoleFilter> personFilters)
        {
            var desiredPairs = personFilters
                .SelectMany(f => f.PersonIds.Select(pid => (PersonId: pid, RoleId: f.RoleId)))
                .ToHashSet();

            var toRemove = book.Participations
                .Where(p => !desiredPairs.Contains((p.PersonId, p.PersonRoleId)))
                .ToList();

            foreach (var participation in toRemove)
                book.Participations.Remove(participation);

            var currentPairs = book.Participations
                .Select(p => (p.PersonId, p.PersonRoleId))
                .ToHashSet();

            foreach (var pair in desiredPairs.Where(dp => !currentPairs.Contains(dp)))
            {
                book.Participations.Add(new BookParticipation
                {
                    PersonId = pair.PersonId,
                    PersonRoleId = pair.RoleId
                });
            }
        }

        //public async Task UpdateAsync(Book book, List<PersonRoleFilter>? personFilters, CancellationToken ct)
        //{
        //    _context.Books.Update(book);

        //    if(personFilters != null)
        //    {
        //        await _context.Entry(book)
        //            .Collection(b => b.Participations)
        //            .LoadAsync(ct);

        //        book.Participations.Clear();

        //        foreach (var roleFilter in personFilters)
        //        {
        //            if (roleFilter.PersonIds == null) continue;

        //            foreach(var personId in roleFilter.PersonIds)
        //            {
        //                book.Participations.Add(new BookParticipation
        //                {
        //                    BookId = book.Id,
        //                    PersonId = personId,
        //                    PersonRoleId = roleFilter.RoleId
        //                });
        //            }
        //        }
        //    }

        //    await _context.SaveChangesAsync(ct);
        //}
    }
}
