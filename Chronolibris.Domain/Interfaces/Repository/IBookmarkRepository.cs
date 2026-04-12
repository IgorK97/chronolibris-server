using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IBookmarkRepository : IGenericRepository<Bookmark>
    {
        Task<List<Bookmark>> GetAllForBookAndUserAsync(long bookId, long userId, CancellationToken token);
        Task<Bookmark?> GetConcreteBookmark(long bookId, long userId, int paraIndex, CancellationToken token);

    }
}
