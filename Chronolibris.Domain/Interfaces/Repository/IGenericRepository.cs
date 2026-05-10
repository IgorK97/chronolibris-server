using System.Linq.Expressions;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IGenericRepository<TEntity> where TEntity:class
    {
        Task<TEntity?> GetByIdAsync(long id, CancellationToken token = default);
        Task<List<TEntity>> GetAllAsync(CancellationToken token = default);
        Task AddAsync(TEntity entity, CancellationToken token = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>
            predicate, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken token = default);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate,  CancellationToken cancellationToken = default);
        Task SaveChangesAsync();
    }
}
