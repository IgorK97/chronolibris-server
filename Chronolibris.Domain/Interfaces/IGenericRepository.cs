using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Interfaces
{
    /// <summary>
    /// Определяет контракт для обобщенного репозитория, предоставляя базовые операции CRUD 
    /// (Create, Read, Update, Delete) для сущностей домена.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности домена, которой управляет репозиторий. 
    /// Должен быть ссылочным типом (<c>class</c>).</typeparam>
    public interface IGenericRepository<TEntity> where TEntity:class
    {

        /// <summary>
        /// Асинхронно получает сущность по ее уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор сущности.</param>
        /// <param name="token">Токен отмены для прерывания операции. По умолчанию — <c>default</c>.</param>
        /// <returns>
        /// Задача, которая представляет асинхронную операцию. Результат задачи — 
        /// сущность <typeparamref name="TEntity"/> или <c>null</c>, если сущность не найдена.
        /// </returns>
        Task<TEntity?> GetByIdAsync(long id, CancellationToken token = default);

        /// <summary>
        /// Асинхронно получает список всех сущностей данного типа.
        /// </summary>
        /// <param name="token">Токен отмены для прерывания операции. По умолчанию — <c>default</c>.</param>
        /// <returns>
        /// Задача, которая представляет асинхронную операцию. Результат задачи — 
        /// коллекция только для чтения (<see cref="System.Collections.Generic.IReadOnlyList{T}"/>) 
        /// всех сущностей <typeparamref name="TEntity"/>.
        /// </returns>
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken token = default);

        /// <summary>
        /// Асинхронно добавляет новую сущность в контекст отслеживания изменений. 
        /// Изменения будут сохранены при вызове метода Unit of Work (<c>SaveChangesAsync</c>).
        /// </summary>
        /// <param name="entity">Сущность, которую нужно добавить.</param>
        /// <param name="token">Токен отмены для прерывания операции. По умолчанию — <c>default</c>.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        Task AddAsync(TEntity entity, CancellationToken token = default);

        /// <summary>
        /// Помечает существующую сущность как измененную (<c>Modified</c>) в контексте отслеживания изменений. 
        /// Изменения будут сохранены при вызове метода Unit of Work.
        /// </summary>
        /// <param name="entity">Сущность, которую нужно обновить.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Помечает существующую сущность как удаленную (<c>Deleted</c>) в контексте отслеживания изменений.
        /// Удаление будет выполнено при вызове метода Unit of Work.
        /// </summary>
        /// <param name="entity">Сущность, которую нужно удалить.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Принудительно отсоединяет указанную сущность от контекста отслеживания изменений (Detached state).
        /// </summary>
        /// <param name="entity">Сущность, которую нужно отсоединить.</param>
        void Detach(TEntity entity);
        Task SaveChangesAsync();
    }
}
