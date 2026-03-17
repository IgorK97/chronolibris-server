using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;

namespace Chronolibris.Domain.Interfaces
{
    /// <summary>
    /// Определяет контракт для репозитория, управляющего сущностями <see cref="Bookmark"/>.
    /// Наследует базовые операции CRUD от <see cref="IGenericRepository{T}"/>.
    /// </summary>
    public interface IBookmarkRepository : IGenericRepository<Bookmark>
    {
        /// <summary>
        /// Асинхронно получает все закладки для указанной книги и конкретного пользователя.
        /// </summary>
        /// <param name="bookId">Идентификатор книги, для которой ищутся закладки.</param>
        /// <param name="userId">Идентификатор пользователя, создавшего закладки.</param>
        /// <param name="token">Токен отмены для прерывания операции.</param>
        /// <returns>
        /// Задача, которая представляет асинхронную операцию. Результат задачи — 
        /// <see cref="System.Collections.Generic.List{T}"/> сущностей <see cref="Bookmark"/>.
        /// </returns>
        Task<List<Bookmark>> GetAllForBookAndUserAsync(long bookId, long userId, CancellationToken token);


    }
}
