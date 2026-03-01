using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;

namespace Chronolibris.Domain.Interfaces
{
    public interface IShelfRepository : ICommonShelvesRepository
    {
        Task<bool> IsInFavorite(long userId, long bookId);
        Task<bool> IsRead(long userId, long bookId);
        Task<bool> IsInShelf(long userId, long shelfId);
        Task<long[]> SeekBookInShelves(long userId, long bookId);
    }
}
