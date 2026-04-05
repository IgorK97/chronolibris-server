using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces
{

    /// <summary>
    /// Определяет контракт для репозитория, управляющего сущностями <see cref="Selection"/> (подборки/коллекции книг).
    /// </summary>
    public interface ISelectionsRepository
    {

        /// <summary>
        /// Асинхронно получает сущность подборки по ее уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор подборки.</param>
        /// <param name="token">Токен отмены для прерывания операции. По умолчанию — <c>default</c>.</param>
        /// <returns>
        /// Задача, которая представляет асинхронную операцию. Результат задачи — 
        /// сущность <see cref="Selection"/> или <c>null</c>, если подборка не найдена.
        /// </returns>
        Task<Selection?> GetByIdAsync(long id, CancellationToken token = default);

        /// <summary>
        /// Асинхронно получает список всех активных подборок, которые должны отображаться пользователю.
        /// </summary>
        /// <param name="token">Токен отмены для прерывания операции. По умолчанию — <c>default</c>.</param>
        /// <returns>
        /// Задача, которая представляет асинхронную операцию. Результат задачи — 
        /// коллекция <see cref="System.Collections.Generic.IEnumerable{T}"/> активных сущностей <see cref="Selection"/>.
        /// </returns>
        Task<IEnumerable<Selection>> GetActiveSelectionsAsync(CancellationToken token = default);
        Task<List<SelectionDetails>> GetSelectionsAsync(
            long? lastId,
            int limit,
            bool? onlyActive, CancellationToken token = default);
        /// <summary>
        /// Асинхронно получает книги, включенные в указанную подборку, с поддержкой пагинации.
        /// </summary>
        /// <param name="selectionId">Идентификатор подборки.</param>
        /// <param name="page">Номер запрашиваемой страницы (начиная с 1).</param>
        /// <param name="pageSize">Количество книг на странице.</param>
        /// <param name="token">Токен отмены для прерывания операции. По умолчанию — <c>default</c>.</param>
        /// <returns>
        /// Кортеж, содержащий коллекцию сущностей <see cref="Book"/> для текущей страницы 
        /// и общее количество книг в подборке (<c>TotalCount</c>).
        /// </returns>
        Task<List<BookListItem>>
            GetBooksForSelection(long selectionId, long? lastId, int limit, long userId, bool mode, CancellationToken token = default);

        Task<long> CreateAsync(Selection selection, CancellationToken ct);
        Task<bool> UpdateAsync(long selectionId, string? name, string? description, bool? isActive, CancellationToken ct);
        Task<bool> AddBookToSelectionAsync(long selectionId, long bookId, CancellationToken ct);
        Task<bool> RemoveBookFromSelectionAsync(long selectionId, long bookId, CancellationToken ct);
        Task<bool> DeleteAsync(long selectionId, CancellationToken ct);
    }

}
