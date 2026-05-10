using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<List<ReviewDetailsWithVote>> GetByBookIdAsync(long bookId, long? lastId, int limit, long? userId, CancellationToken token = default);
        Task<ReviewDetailsWithVote?> GetByIdWithVotesAsync(long reviewId, long userId, CancellationToken token = default);
        Task<ReviewDetailsWithVote?> GetActiveByUserAndBookAsync(long userId, long bookId, CancellationToken token = default);

    }
}
