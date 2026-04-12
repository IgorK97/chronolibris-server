using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<CommentDto?> GetByIdWithVotesAsync(long commentId, long userId, CancellationToken token);
        Task<List<CommentDto>> GetRootCommentsByBookIdAsync(long bookId, long? lastId, int limit, long userId, CancellationToken token);
        Task<List<CommentDto>> GetRepliesByParentIdAsync(long parentCommentId, long? lastId, int limit, long userId, CancellationToken token);
    }
}
