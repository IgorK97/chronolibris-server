using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface ICommentReactionsRepository : IGenericRepository<CommentReactions>
    {
        Task<CommentReactions?> GetCommentReactionByUserIdAsync(long commentId, long userId, CancellationToken token = default);
    }
}
