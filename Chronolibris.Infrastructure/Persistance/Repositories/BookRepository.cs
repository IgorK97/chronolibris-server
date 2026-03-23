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
        private readonly DbSet<Book> _set;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BookRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения, используемый для доступа к данным.</param>
        public BookRepository(ApplicationDbContext context) : base(context) { _set = context.Set<Book>(); }

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
                        AverageRating = b.Reviews.Any(r => r.ReviewStatusId == 2) ? b.Reviews
                                        .Where(r => r.ReviewStatusId == 2)
                                        .Average(r => (decimal)r.Score) : 0M,
                        RatingsCount = b.Reviews.Count(r => r.ReviewStatusId==2),
                        ReviewsCount = b.Reviews.Count(r => r.ReviewText != null && r.ReviewStatusId==2),
                        UserRating = b.Reviews.Where(r => r.UserId == userId && r.ReviewStatusId == 2)
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

        public async Task<Book?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _set
                .Include(b => b.Country)
                .Include(b => b.Language)
                .Include(b => b.Publisher)
                .Include(b => b.Series)
                .Include(b => b.Participations)
                    .ThenInclude(p => p.Person)
                    .Include(b => b.BookContents)
                    .ThenInclude(bc => bc.Content)
                        .ThenInclude(c => c.Themes)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _set.ToListAsync(cancellationToken);
        }

        public async Task<(List<Book> Items, int TotalCount, string? NextCursor, string? PrevCursor)> GetWithFilterAsync(
            BookFilterRequest filter, CancellationToken cancellationToken = default)
        {
            var query = _set
                .Include(b => b.Country)
                .Include(b => b.Language)
                .Include(b => b.Publisher)
                .Include(b => b.Series)
                .Include(b => b.Participations)
                    .ThenInclude(p => p.Person)
                .Include(b=>b.BookContents)
                    .ThenInclude(bc=>bc.Content)
                        .ThenInclude(c=>c.Themes)
                .AsQueryable();

            // Поиск по названию
            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            {
                query = query.Where(b => b.Title.Contains(filter.SearchQuery));
            }

            // Фильтр по автору
            if (!string.IsNullOrWhiteSpace(filter.AuthorName))
            {
                query = query.Where(b => b.Participations.Any(p =>
                    p.Person.Name.Contains(filter.AuthorName)));
            }

            // Включение тем
            //if (filter.IncludeThemeIds != null && filter.IncludeThemeIds.Any())
            //{
            //    query = query.Where(b => b.BookContents.Content.Themes.Any(t =>
            //        filter.IncludeThemeIds.Contains(t.Id)));
            //}

            // Включение тем (через BookContent → Content → Themes)
            if (filter.IncludeThemeIds != null && filter.IncludeThemeIds.Any())
            {
                query = query.Where(b => b.BookContents.Any(bc =>
                    bc.Content.Themes.Any(t => filter.IncludeThemeIds.Contains(t.Id))));
            }

            // Исключение тем
            //if (filter.ExcludeThemeIds != null && filter.ExcludeThemeIds.Any())
            //{
            //    query = query.Where(b => !b.Themes.Any(t =>
            //        filter.ExcludeThemeIds.Contains(t.Id)));
            //}
            if (filter.ExcludeThemeIds != null && filter.ExcludeThemeIds.Any())
            {
                query = query.Where(b => !b.BookContents.Any(bc =>
                    bc.Content.Themes.Any(t => filter.ExcludeThemeIds.Contains(t.Id))));
            }

            // Фильтр по издательству
            if (filter.PublisherId.HasValue)
            {
                query = query.Where(b => b.PublisherId == filter.PublisherId.Value);
            }

            // Фильтр по серии
            if (filter.SeriesId.HasValue)
            {
                query = query.Where(b => b.SeriesId == filter.SeriesId.Value);
            }

            // Фильтр по языку
            if (filter.LanguageId.HasValue)
            {
                query = query.Where(b => b.LanguageId == filter.LanguageId.Value);
            }

            // Фильтр по году
            if (filter.YearFrom.HasValue)
            {
                query = query.Where(b => b.Year >= filter.YearFrom.Value);
            }
            if (filter.YearTo.HasValue)
            {
                query = query.Where(b => b.Year <= filter.YearTo.Value);
            }

            // Фильтр по доступности
            if (filter.IsAvailable.HasValue)
            {
                query = query.Where(b => b.IsAvailable == filter.IsAvailable.Value);
            }


            // Получаем общее количество
            var totalCount = await query.CountAsync(cancellationToken);

            // Курсорная пагинация
            if (!string.IsNullOrWhiteSpace(filter.Cursor))
            {
                if (long.TryParse(filter.Cursor, out var cursorId))
                {
                    query = query.Where(b => b.Id > cursorId);
                }
            }

            query = query.OrderBy(b => b.Id);

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

        public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
        {
            await _set.AddAsync(book, cancellationToken);
        }

        public void Update(Book book)
        {
            _set.Update(book);
        }

        public void Delete(Book book)
        {
            _set.Remove(book);
        }

        public async Task<List<string>> GetAuthorNamesByBookIdAsync(long bookId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BookParticipation>()
                .Include(bp => bp.Person)
                .Where(bp => bp.BookId == bookId)
                .Select(bp => bp.Person.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Theme>> GetThemesByBookIdAsync(long bookId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BookContent>()
                .Include(bc => bc.Content)
                    .ThenInclude(c => c.Themes)
                .Where(bc => bc.BookId == bookId)
                .SelectMany(bc => bc.Content.Themes)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsLinkedToContentAsync(long bookId, long contentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BookContent>()
                .AnyAsync(bc => bc.BookId == bookId && bc.ContentId == contentId, cancellationToken);
        }

        public async Task<int> GetContentsCountAsync(long bookId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BookContent>()
                .CountAsync(bc => bc.BookId == bookId, cancellationToken);
        }

        public async Task<List<Content>> GetContentsByBookIdAsync(long bookId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BookContent>()
                .Include(bc => bc.Content)
                    .ThenInclude(c => c.Country)
                .Include(bc => bc.Content)
                    .ThenInclude(c => c.Language)
                .Include(bc => bc.Content)
                    .ThenInclude(c => c.ContentType)
                .Include(bc => bc.Content)
                    .ThenInclude(c => c.Themes)
                .Include(bc => bc.Content)
                    .ThenInclude(c => c.Participations)
                        .ThenInclude(cp => cp.Person)
                .Where(bc => bc.BookId == bookId)
                //.OrderBy(bc => bc.Order)
                .Select(bc => bc.Content)
                .ToListAsync(cancellationToken);
        }

        public async Task LinkContentToBookAsync(long bookId, long contentId, int order, CancellationToken cancellationToken = default)
        {
            var bookContent = new BookContent
            {
                BookId = bookId,
                ContentId = contentId,
                //Order = order
            };

            await _context.Set<BookContent>().AddAsync(bookContent, cancellationToken);
        }

        public async Task UnlinkContentFromBookAsync(long bookId, long contentId, CancellationToken cancellationToken = default)
        {
            var bookContent = await _context.Set<BookContent>()
                .FirstOrDefaultAsync(bc => bc.BookId == bookId && bc.ContentId == contentId, cancellationToken);

            if (bookContent != null)
            {
                _context.Set<BookContent>().Remove(bookContent);
            }
        }


    }
}
