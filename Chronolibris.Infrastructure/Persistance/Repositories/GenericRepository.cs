using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.Persistance.Repositories
{
    /// <summary>
    /// Обобщенная реализация репозитория, предоставляющая базовые операции CRUD 
    /// (Create, Read, Update, Delete) для сущностей домена с использованием Entity Framework Core.
    /// Реализует интерфейс <see cref="IGenericRepository{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности домена, которой управляет репозиторий. Должен быть ссылочным типом.</typeparam>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity:class
    {
        /// <summary>
        /// Контекст базы данных Entity Framework Core. Используется для доступа к БД и отслеживания изменений.
        /// </summary>
        protected readonly ApplicationDbContext _context;

        /// <summary>
        /// Коллекция сущностей (<see cref="DbSet{TEntity}"/>) в контексте базы данных.
        /// </summary>
        protected readonly DbSet<TEntity> _set;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GenericRepository{TEntity}"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения, используемый для доступа к данным.</param>
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _set = context.Set<TEntity>();
        }

        /// <summary>
        /// Асинхронно получает сущность по ее уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор сущности (предполагается первичный ключ).</param>
        /// <param name="token">Токен отмены для прерывания операции.</param>
        /// <returns>
        /// Задача, которая представляет асинхронную операцию. Результат задачи — 
        /// сущность <typeparamref name="TEntity"/> или <c>null</c>, если сущность не найдена.
        /// </returns>
        public async Task<TEntity?> GetByIdAsync(long id, CancellationToken token) =>
        await _set.FindAsync(id, token);

        /// <summary>
        /// Асинхронно получает список всех сущностей данного типа.
        /// </summary>
        /// <param name="token">Токен отмены для прерывания операции.</param>
        /// <returns>
        /// Задача, которая представляет асинхронную операцию. Результат задачи — 
        /// коллекция только для чтения (<see cref="System.Collections.Generic.IReadOnlyList{T}"/>) всех сущностей.
        /// </returns>
        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken token) =>
            await _set.ToListAsync(token);

        /// <summary>
        /// Асинхронно добавляет новую сущность в контекст отслеживания изменений.
        /// Операция I/O будет выполнена при вызове <c>IUnitOfWork.SaveChangesAsync</c>.
        /// </summary>
        /// <param name="entity">Сущность, которую нужно добавить.</param>
        /// <param name="token">Токен отмены для прерывания операции.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        public async Task AddAsync(TEntity entity, CancellationToken token) =>
            await _set.AddAsync(entity, token);

        /// <summary>
        /// Помечает существующую сущность как измененную (<c>Modified</c>) в контексте отслеживания изменений.
        /// Операция I/O будет выполнена при вызове <c>IUnitOfWork.SaveChangesAsync</c>.
        /// </summary>
        /// <param name="entity">Сущность, которую нужно обновить.</param>
        public void Update(TEntity entity) =>
            _set.Update(entity);

        /// <summary>
        /// Помечает существующую сущность как удаленную (<c>Deleted</c>) в контексте отслеживания изменений.
        /// Операция I/O будет выполнена при вызове <c>IUnitOfWork.SaveChangesAsync</c>.
        /// </summary>
        /// <param name="entity">Сущность, которую нужно удалить.</param>
        public void Delete(TEntity entity) =>
            _set.Remove(entity);

        /// <summary>
        /// Реализация Detach. Устанавливает состояние сущности в EntityState.Detached.
        /// </summary>
        /// <param name="entity">Сущность для отсоединения.</param>
        public void Detach(TEntity entity)
        {
            // Получаем объект Entry (запись отслеживания) для данной сущности
            var entityEntry = _context.Entry(entity);

            // Если сущность отслеживается (ее состояние не Detached), 
            // принудительно устанавливаем ее состояние в Detached.
            if (entityEntry.State != EntityState.Detached)
            {
                entityEntry.State = EntityState.Detached;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
