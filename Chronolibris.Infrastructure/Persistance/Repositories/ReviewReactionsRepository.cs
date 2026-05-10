using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.Persistance.Repositories
{
    public class ReviewReactionsRepository : GenericRepository<ReviewReactions>, IReviewReactionsRepository
    {
        public ReviewReactionsRepository(ApplicationDbContext context) : base(context) { }
        public async Task<ReviewReactions?> GetReviewReactionByUserIdAsync(long reviewId, long userId, CancellationToken token)
        {
            return await _context.ReviewReactions.FirstOrDefaultAsync(rr => rr.UserId == userId && rr.ReviewId==reviewId, token);
        }
    }
}
