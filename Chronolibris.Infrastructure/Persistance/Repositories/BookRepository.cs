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
                .Where(c => c.BookContents.Any(bc => bc.BookId == bookId))
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
                .Where(b => b.Id == bookId)
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
                    HasHistoricalVersions = b.HasHistoricalVersions,
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
                HasHistoricalVersions = raw.HasHistoricalVersions,
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

        //public override async Task<List<Book>> GetAllAsync(CancellationToken cancellationToken = default)
        //{
        //    return await _context.Books.ToListAsync(cancellationToken); //или может вообще исключение выбрасывать, если кто-то попытается
        //сразу все книги загрузить
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


            await _context.Books.AddAsync(book, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return book.Id;
        }

        //public void Update(Book book)
        //{
        //    _context.Books.Update(book);
        //}

        //public void Delete(Book book)
        //{
        //    _context.Books.Remove(book);
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
    }
}
