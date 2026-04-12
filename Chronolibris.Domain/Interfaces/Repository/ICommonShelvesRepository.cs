using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface ICommonShelvesRepository : IGenericRepository<Shelf>
    {
        Task<Shelf?> GetByIdAsync(long shelfId, CancellationToken token = default);
        Task<IEnumerable<Shelf>> GetForUserAsync(long userId, CancellationToken token = default);

        Task<List<BookListItem>>
                    GetBooksForShelfAsync(long shelfId, long? lastId, int limit, long userId, CancellationToken ct = default);

        Task AddBookToShelf(long shelfId, long bookId, CancellationToken token = default);
        Task RemoveBookFromShelf(long shelfId, long bookId, CancellationToken token = default);
    }

}
