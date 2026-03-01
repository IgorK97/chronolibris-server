using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<Book?> GetBookWithRelationsAsync(long id, CancellationToken token)
        {
            return await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Series)
                .Include(b => b.Country)
                .Include(b => b.Language)

                .Include(b => b.Participations)
                    .ThenInclude(p => p.Person)
                .Include(b => b.Participations)
                    .ThenInclude(p => p.PersonRole)

                .Include(b => b.BookContents)
                    .ThenInclude(bc => bc.Content)
                        .ThenInclude(c => c.Participations)
                            .ThenInclude(p => p.Person)
                .Include(b => b.BookContents)
                    .ThenInclude(bc => bc.Content)
                        .ThenInclude(c => c.Participations)
                            .ThenInclude(p => p.PersonRole)


                .Include(b => b.BookContents)
                    .ThenInclude(bc => bc.Content)
                        .ThenInclude(c => c.Themes)

                //.Include(b => b.Persons)
                .FirstOrDefaultAsync(b => b.Id == id, token);
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
                    AverageRating = b.AverageRating,
                    CoverUri = b.CoverPath,
                    RatingsCount = b.RatingsCount,
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
