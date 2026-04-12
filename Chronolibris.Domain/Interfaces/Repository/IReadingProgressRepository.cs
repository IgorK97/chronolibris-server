using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IReadingProgressRepository : IGenericRepository<ReadingProgress>
    {
        Task<ReadingProgress?> GetForBookUser(long bookId, long userId, CancellationToken token = default);
        Task<List<BookListItem>> GetBooks(long userId, long? lastId, int limit, CancellationToken ct);
    }
}
