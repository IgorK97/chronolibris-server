using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса для получения списка закладок (<see cref="BookmarkDetails"/>) для конкретной книги и пользователя.
    /// Использует первичный конструктор для внедрения зависимости <see cref="IBookmarkRepository"/>.
    /// Реализует интерфейс <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// для обработки <see cref="GetBookmarksQuery"/> и возврата списка <see cref="BookmarkDetails"/>.
    /// </summary>
    public class GetBookmarksHandler(IBookmarkRepository bookmarkRepository) : IRequestHandler<GetBookmarksQuery, List<BookmarkDetails>>
    {
        // Примечание: Внедрение зависимости через первичный конструктор(Primary Constructor)
        // автоматически создает приватное поле только для чтения `bookmarkRepository`.

        /// <summary>
        /// Обрабатывает запрос на получение закладок.
        /// </summary>
        /// <remarks>
        /// 1. Вызывает репозиторий для получения всех закладок, соответствующих <c>BookId</c> и <c>UserId</c>.
        /// 2. Если закладки не найдены (возвращен <c>null</c>), возвращает пустой список DTO.
        /// 3. Преобразует сущности закладок в <see cref="BookmarkDetails"/> DTO.
        /// </remarks>
        /// <param name="request">Запрос, содержащий идентификаторы <c>Bookid</c> и <c>UserId</c>.</param>
        /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию.
        /// Результат задачи — список <see cref="BookmarkDetails"/>, содержащий детали закладок, 
        /// или пустой список, если закладки не найдены.
        /// </returns>
        public async Task<List<BookmarkDetails>> Handle(GetBookmarksQuery request, CancellationToken cancellationToken)
        {
            var bookmarks = await bookmarkRepository
                                            .GetAllForBookAndUserAsync(request.Bookid, request.UserId, cancellationToken);


            if (bookmarks == null)
            {
                return new List<BookmarkDetails>();
            }

            // Маппинг сущностей Bookmark на DTO BookmarkDetails
            return bookmarks.Select(b => new BookmarkDetails
            {
                Id = b.Id,
                Note = b.Note,
                BookFileId=b.BookFileId,
                ParaIndex = b.ParaIndex,
                CreatedAt = b.CreatedAt
            }).ToList();
        }
    }
}
