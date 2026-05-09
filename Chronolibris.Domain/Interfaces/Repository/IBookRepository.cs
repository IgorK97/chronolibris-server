using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        void SyncParticipations(Book book, List<PersonRoleFilter> personFilters);
        Task<List<Content>> GetContentsWithDetailsByBookIdAsync(long bookId, CancellationToken ct);
        Task<long> CreateAsync(Book book, List<PersonRoleFilter>? personFilters, CancellationToken cancellationToken = default);
        Task<BookDetails?> GetBookWithRelationsAsync(long bookId, long userId, bool mode, CancellationToken token = default);
    }
}
