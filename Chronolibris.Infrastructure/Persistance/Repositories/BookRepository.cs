using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using Chronolibris.Domain.SystemConstants;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.Persistance.Repositories
{
    /// <summary>
    /// Репозиторий для управления сущностями книг (<see cref="Book"/>). 
    /// Предоставляет специализированные методы доступа к данным, расширяя <see cref="GenericRepository{TEntity}"/>.
    /// Реализует интерфейс <see cref="IBookRepository"/>.
    /// </summary>
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BookRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения, используемый для доступа к данным.</param>
        public BookRepository(ApplicationDbContext context) : base(context) { }

        /// <summary>
        /// Асинхронно получает полную сущность книги по ее идентификатору, 
        /// включая все связанные сущности (Relations) с помощью Eager Loading.
        /// </summary>
        /// <remarks>
        /// Этот метод использует множество вызовов <c>Include</c> для загрузки Publisher, Series, Country, 
        /// Language, BookContents, Participations и Persons, предотвращая проблемы N+1.
        /// </remarks>
        /// <param name="id">Уникальный идентификатор книги.</param>
        /// <param name="token">Токен отмены для прерывания запроса.</param>
        /// <returns>
        /// Задача, которая представляет асинхронную операцию. Результат задачи — 
        /// сущность <see cref="Book"/> со всеми загруженными связями или <c>null</c>, если книга не найдена.
        /// </returns>
        //public async Task<BookDetails?> GetBookWithRelationsAsync(long bookId,
        //    long userId, CancellationToken token)
        //{
        //    var dto = await _context.Books
        //        .Where(b => b.Id == bookId)
        //        .Select(b => new BookDetails
        //        {
        //            Id = b.Id,
        //            Title = b.Title,
        //            Description = b.Description,
        //            Year = b.Year,
        //            ISBN = b.ISBN,
        //            IsAvailable = b.IsAvailable,
        //            IsReviewable = b.IsReviewable,
        //            CoverUri = b.CoverPath,
        //            Country = b.Country.Name,
        //            Language = b.Language.Name,
        //            Publisher = b.Publisher != null
        //            ? new PublisherDetails { Id = (long)b.PublisherId, Name = b.Publisher.Name } : null,
        //            AverageRating = b.IsReviewable && b.Reviews.Any()
        //            ? b.Reviews.Average(r => (decimal)r.Score)
        //            : 0M,
        //            RatingsCount = b.IsReviewable ? b.Reviews.Count() : 0,
        //            ReviewsCount = b.IsReviewable ? b.Reviews.Count(r => r.ReviewText != null) : 0,
        //            UserRating = b.IsReviewable ? b.Reviews.Where(r => r.UserId == userId).Select(r => (decimal?)r.Score).FirstOrDefault() ?? 0M : 0M,

        //            IsFavorite = b.Shelves.Any(s => s.UserId == userId && s.ShelfTypeId == ShelfTypes.FAVORITES_CODE),
        //            IsRead = b.Shelves.Any(s => s.UserId == userId && s.ShelfTypeId == ShelfTypes.READ_CODE),
        //            Participants = b.Participations
        //            .Select(p => new { p.PersonRoleId, RoleName = p.PersonRole.Name, p.PersonId, p.Person.Name })
        //            .Concat(b.BookContents
        //                .SelectMany(bc => bc.Content.Participations)
        //                .Select(p => new { p.PersonRoleId, RoleName = p.PersonRole.Name, p.PersonId, p.Person.Name })
        //            )
        //            .GroupBy(p => p.PersonRoleId)
        //            .Select(g => new BookPersonGroupDetails
        //            {
        //                Role = g.Key,

        //                Persons = g.GroupBy(p => p.PersonId).Select(pg => new PersonDetails
        //                {
        //                    Id = pg.Key,
        //                    FullName = pg.First().Name
        //                }).ToList()
        //            }).ToList(),
        //            Themes = b.BookContents
        //                .SelectMany(bc => bc.Content.Themes)
        //                .Select(t => new ThemeDetails { Id = t.Id, Name = t.Name })
        //                .Distinct()
        //                .ToList()
        //        }).FirstOrDefaultAsync(token);
        //    return dto;



        //    //return await _context.Books
        //    //    .Include(b => b.Publisher)
        //    //    .Include(b => b.Series)
        //    //    .Include(b => b.Country)
        //    //    .Include(b => b.Language)

        //    //    .Include(b => b.Participations)
        //    //        .ThenInclude(p => p.Person)
        //    //    .Include(b => b.Participations)
        //    //        .ThenInclude(p => p.PersonRole)

        //    //    .Include(b => b.BookContents)
        //    //        .ThenInclude(bc => bc.Content)
        //    //            .ThenInclude(c => c.Participations)
        //    //                .ThenInclude(p => p.Person)
        //    //    .Include(b => b.BookContents)
        //    //        .ThenInclude(bc => bc.Content)
        //    //            .ThenInclude(c => c.Participations)
        //    //                .ThenInclude(p => p.PersonRole)


        //    //    .Include(b => b.BookContents)
        //    //        .ThenInclude(bc => bc.Content)
        //    //            .ThenInclude(c => c.Themes)

        //    //    //.Include(b => b.Persons)
        //    //    .FirstOrDefaultAsync(b => b.Id == id, token);
        //}

        public async Task<BookDetails?> GetBookWithRelationsAsync(long bookId, long userId, CancellationToken token)
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
                    CountryName = b.Country.Name,
                    LanguageName = b.Language.Name,
                    PublisherId = b.PublisherId,
                    PublisherName = b.Publisher != null ? b.Publisher.Name : null,
                    Stats = b.IsReviewable ? new
                    {
                        AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => (decimal)r.Score) : 0M,
                        RatingsCount = b.Reviews.Count(),
                        ReviewsCount = b.Reviews.Count(r => r.ReviewText != null),
                        UserRating = b.Reviews.Where(r => r.UserId == userId).Select(r => (decimal?)r.Score).FirstOrDefault()
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
                        .Select(t => new { t.Id, t.Name })
                })
                .FirstOrDefaultAsync(token);

            if (raw == null) return null;

            var allParticipations = raw.DirectParticipations
                .Concat(raw.ContentParticipations);

            return new BookDetails
            {
                Id = raw.Id,
                Title = raw.Title,
                Description = raw.Description,
                Year = raw.Year,
                ISBN = raw.ISBN,
                IsAvailable = raw.IsAvailable,
                IsReviewable = raw.IsReviewable,
                CoverUri = raw.CoverPath,
                Country = raw.CountryName,
                Language = raw.LanguageName,
                Publisher = raw.PublisherName != null
                    ? new PublisherDetails { Id = (long)raw.PublisherId!, Name = raw.PublisherName }
                    : null,
                AverageRating = raw.Stats?.AverageRating ?? 0M,
                RatingsCount = raw.Stats?.RatingsCount??0,
                ReviewsCount = raw.Stats?.ReviewsCount??0,
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
                    .ToList()
            };
        }

        public async Task<List<BookListItem>>
            GetSearchedBooks(string query, long? lastId, int limit, long userId, CancellationToken token)
        {
            var request = _context.Books.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var term = query.Trim();
                request = request.Where(b =>
                    b.Title.Contains(term) ||
                    b.BookContents.Any(bc =>
                        bc.Content.Participations.Any(p =>
                            p.Person.Name.Contains(term)
                        )
                    )
                );
            }

            if (lastId.HasValue)
            {
                request = request.Where(b => b.Id > lastId.Value);
            }

       

            var books = await request
                .OrderBy(rp => rp.Id)
                .Select(b => new BookListItem
                {
                    Id = b.Id,
                    Title = b.Title,
                    CoverUri = b.CoverPath,
                    IsReviewable = b.IsReviewable,
                    AverageRating = b.Reviews.Any() ? b.Reviews.Average(r=>(decimal)r.Score): 0.0M,
                    RatingsCount = b.IsReviewable ? b.Reviews.Count() : 0,
                    IsFavorite = b.Shelves.Any(s =>
                        s.UserId == userId &&
                        s.ShelfType.Code == ShelfTypes.FAVORITES),

                    IsRead = b.Shelves.Any(s =>
                        s.UserId == userId &&
                        s.ShelfType.Code == ShelfTypes.READ),

                    Authors = b.BookContents
                        .SelectMany(bc => bc.Content.Participations
                            .Select(p => p.Person.Name))
                        .ToList()
                })
                .Take(limit + 1)
                .ToListAsync(token);

            return books;
        }

        public async Task<Content?> GetContentWithRelationsAsync(long id, CancellationToken token)
        {
            return await _context.Contents
                .Include(b => b.Country)
                .Include(b => b.Language)
                .Include(b => b.ContentType)

                .Include(b => b.Participations)
                    .ThenInclude(p => p.Person)
                .Include(b => b.Participations)
                    .ThenInclude(p => p.PersonRole)

                .Include(b => b.BookContents)
                    .ThenInclude(bc => bc.Book)
                        .ThenInclude(c => c.Participations)
                            .ThenInclude(p => p.Person)
                .Include(b => b.BookContents)
                    .ThenInclude(bc => bc.Book)
                        .ThenInclude(c => c.Participations)
                            .ThenInclude(p => p.PersonRole)

                .Include(c => c.Themes)

                //.Include(b => b.Persons)
                .FirstOrDefaultAsync(b => b.Id == id, token);
        }
    }
}
