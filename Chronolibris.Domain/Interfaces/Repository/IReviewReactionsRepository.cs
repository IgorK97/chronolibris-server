using Chronolibris.Domain.Entities;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IReviewReactionsRepository : IGenericRepository<ReviewReactions>
    {
        Task<ReviewReactions?> GetReviewReactionByUserIdAsync(long reviewId, long userId, CancellationToken token = default);
    }
}
