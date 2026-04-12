using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Interfaces.Repository
{

    public interface IGenericRepository<TEntity> where TEntity:class
    {
        Task<TEntity?> GetByIdAsync(long id, CancellationToken token = default);
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken token = default);
        Task AddAsync(TEntity entity, CancellationToken token = default);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void Detach(TEntity entity);
        Task SaveChangesAsync();
    }
}
