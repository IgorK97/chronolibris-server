using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Infrastructure.Data;
using Chronolibris.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    public class CommentReactionsRepository : GenericRepository<CommentReactions>, ICommentReactionsRepository
    {
        public CommentReactionsRepository(ApplicationDbContext context) : base(context) { }
        public async Task<CommentReactions?> GetCommentReactionByUserIdAsync(long commentId, long userId, CancellationToken token)
        {
            return await _context.CommentReactions.FirstOrDefaultAsync(rr => rr.UserId == userId && rr.CommentId == commentId, token);
        }
    }
}
