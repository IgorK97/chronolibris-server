using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Interfaces.Repository;
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
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _set;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _set = context.Set<TEntity>();
        }
        public virtual async Task<TEntity?> GetByIdAsync(long id, CancellationToken token) =>
        await _set.FindAsync(id, token);
        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken token) =>
            await _set.ToListAsync(token);
        public virtual async Task AddAsync(TEntity entity, CancellationToken token) =>
            await _set.AddAsync(entity, token);
        public void Update(TEntity entity) =>
            _set.Update(entity);
        public void Delete(TEntity entity) =>
            _set.Remove(entity);
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
