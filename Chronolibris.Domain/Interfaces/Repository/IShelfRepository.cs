using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IShelfRepository : IGenericRepository<Shelf>
    {
        Task<IEnumerable<Shelf>> GetForUserAsync(long userId, CancellationToken token = default);

        Task<List<BookListItem>>
                    GetBooksForShelfAsync(long shelfId, long? lastId, int limit, long userId, CancellationToken ct = default);

        Task AddBookToShelf(long shelfId, long bookId, CancellationToken token = default);
        Task RemoveBookFromShelf(long shelfId, long bookId, CancellationToken token = default);
        Task<bool> IsInShelf(long bookId, long shelfId);
        Task<long[]> SeekBookInShelves(long userId, long bookId);

    }
}