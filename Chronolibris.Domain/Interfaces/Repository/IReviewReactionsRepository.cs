using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IReviewReactionsRepository : IGenericRepository<ReviewReactions>
    {
        Task<ReviewReactions?> GetReviewReactionByUserIdAsync(long reviewId, long userId, CancellationToken token = default);
    }
}
