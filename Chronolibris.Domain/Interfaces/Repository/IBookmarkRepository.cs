using Chronolibris.Domain.Entities;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IBookmarkRepository : IGenericRepository<Bookmark>
    {
        Task<List<Bookmark>> GetAllForBookAndUserAsync(long bookId, long userId, CancellationToken token);
        Task<Bookmark?> GetConcreteBookmark(long bookId, long userId, string xpointer, CancellationToken token);
        Task<(List<Bookmark> Items, int TotalCount)> GetPagedForUserAsync(
            long userId,
            int page,
            int pageSize,
            string? searchQuery,
            CancellationToken token);
    }
}
