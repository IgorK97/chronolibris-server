using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using Chronolibris.Infrastructure.Data;
using Chronolibris.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<CommentDto?> GetByIdWithVotesAsync(long reviewId, long userId, CancellationToken token = default)
        {
            return await _context.Comments.AsNoTracking()
                .Where(r => r.Id == reviewId)
               .Join(_context.Users, c => c.UserId, u => u.Id, (c, u) => new CommentDto
               {
                   CreatedAt = c.CreatedAt,
                   DeletedAt = c.DeletedAt,
                   Id = c.Id,
                   Text = c.Text,
                   UserLogin = u.UserName,
                   ParentCommentId = c.ParentCommentId,
                   RepliesCount = c.Replies.Count(),
                   DislikesCount = c.CommentReactions.LongCount(rr => rr.ReactionType == -1),
                   LikesCount = c.CommentReactions.LongCount(rr => rr.ReactionType == 1),
                   UserVote =c.CommentReactions.Where(rr => rr.UserId == userId)
                        .Select(rr => rr.ReactionType == 1 ? (bool?)true : (rr.ReactionType == 0 ? null : (bool?)false))
                        .FirstOrDefault()
               })
                .FirstOrDefaultAsync(token);
        }

        public async Task<List<CommentDto>> GetRootCommentsByBookIdAsync(long bookId, long? lastId, int limit, long userId, CancellationToken token)
        {
            var query = _context.Comments
                .AsNoTracking()
                .Where(c => c.BookId == bookId && c.ParentCommentId == null);

            if (lastId.HasValue)
                query = query.Where(c => c.Id < lastId.Value);

            var resultQuery = query.OrderByDescending(c => c.Id).Take(limit);

            return await resultQuery.Join(_context.Users, c => c.UserId, u => u.Id, (c, u) => new CommentDto
            {
                Id = c.Id,
                CreatedAt = c.CreatedAt,
                DeletedAt = c.DeletedAt,
                ParentCommentId = c.ParentCommentId,
                Text = c.DeletedAt==null ? c.Text : null,
                RepliesCount = c.Replies.Count(),
                UserLogin = c.DeletedAt==null ? u.UserName : null,
                DislikesCount = c.CommentReactions.LongCount(rr => rr.ReactionType == -1),
                LikesCount = c.CommentReactions.LongCount(rr => rr.ReactionType == 1),
                UserVote = c.CommentReactions.Where(rr => rr.UserId == userId)
                        .Select(rr => rr.ReactionType == 1 ? (bool?)true : (rr.ReactionType == 0 ? null : (bool?)false))
                        .FirstOrDefault()
            }).ToListAsync(token);
        }

        public async Task<List<CommentDto>> GetRepliesByParentIdAsync(long parentCommentId, long? lastId, int limit, long userId, CancellationToken token)
        {
            var query = _context.Comments
                .AsNoTracking()
                .Where(c => c.ParentCommentId == parentCommentId);

            if (lastId.HasValue)
                query = query.Where(c => c.Id < lastId.Value);

            return await query
                .OrderByDescending(c => c.Id)
                .Take(limit)
                .Join(_context.Users, c => c.UserId, u => u.Id, (c, u) => new CommentDto
                {
                    Id = c.Id,
                    CreatedAt = c.CreatedAt,
                    DeletedAt = c.DeletedAt,
                    ParentCommentId = c.ParentCommentId,
                    Text = c.DeletedAt == null ? c.Text : null,
                    RepliesCount = c.Replies.Count(),
                    UserLogin = c.DeletedAt == null ? u.UserName : null,
                    DislikesCount = c.CommentReactions.LongCount(rr => rr.ReactionType == -1),
                    LikesCount = c.CommentReactions.LongCount(rr => rr.ReactionType == 1),
                    UserVote = c.CommentReactions.Where(rr => rr.UserId == userId)
                        .Select(rr => rr.ReactionType == 1 ? (bool?)true : (rr.ReactionType == 0 ? null : (bool?)false))
                        .FirstOrDefault()
                })
                .ToListAsync(token);
        }
    }
}
