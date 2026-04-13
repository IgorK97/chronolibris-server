using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.Persistance.Repositories
{

    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {

        public ReviewRepository(ApplicationDbContext context) : base(context) { }

        public async Task<ReviewDetailsWithVote?> GetActiveByUserAndBookAsync(long userId, long bookId, CancellationToken token = default)
        {
            return await _context.Reviews.AsNoTracking()
                .Where(r => r.UserId == userId && r.BookId == bookId && !r.IsDeleted).OrderByDescending(r => r.CreatedAt)

                .Join(_context.Users, r => r.UserId, u => u.Id, (r, u) => new ReviewDetailsWithVote
                {
                    Review = new Review
                    {
                        Id = r.Id,
                        UserId = r.UserId,
                        BookId = r.BookId,
                        ReviewText = r.ReviewText,
                        Score = r.Score,
                        //ReviewStatusId = r.ReviewStatusId,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        //ModeratedAt = r.ModeratedAt,
                        DeletedAt = r.DeletedAt,
                        IsDeleted= r.IsDeleted,
                        //ReviewStatus = r.ReviewStatus
                    },
                    UserName = u.UserName,
                    DislikesCount = r.ReviewsRatings.LongCount(rr => rr.ReactionType == -1),
                    LikesCount = r.ReviewsRatings.LongCount(rr => rr.ReactionType == 1),
                    UserVote = r.ReviewsRatings.Where(rr => rr.UserId == userId)
                        .Select(rr => rr.ReactionType == 1 ? (bool?) true : (rr.ReactionType == 0 ? null : (bool?) false))
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(token);
        }

        public async Task<List<ReviewDetailsWithVote>> GetByBookIdAsync(long bookId, long? lastId, int limit, long? userId, CancellationToken token)
        {

            var query = _context.Reviews.AsNoTracking()
                .Where(r => r.BookId == bookId);

            if (lastId.HasValue)
            {
                query = query.Where(r => r.Id > lastId.Value);
            }

            return await query.Where(r => !r.IsDeleted && r.ReviewText!= null).OrderBy(r => r.Id).Take(limit+1)
                .Join(_context.Users, r => r.UserId, u => u.Id, (r, u) => new ReviewDetailsWithVote
                {
                    Review = r,
                    UserName = u.UserName,
                    DislikesCount = r.ReviewsRatings.LongCount(rr => rr.ReactionType == -1),
                    LikesCount = r.ReviewsRatings.LongCount(rr => rr.ReactionType == 1),
                    UserVote = (userId == null) ? null : r.ReviewsRatings.Where(rr => rr.UserId == userId)
                        .Select(rr => rr.ReactionType == 1 ? (bool?)true : (rr.ReactionType == 0 ? null : (bool?)false))
                        .FirstOrDefault()
                }).ToListAsync(token);
        }

        public async Task<ReviewDetailsWithVote?> GetByIdWithVotesAsync(long reviewId, long userId, CancellationToken token = default)
        {
            return await _context.Reviews.AsNoTracking()
                .Where(r => r.Id == reviewId)
               .Join(_context.Users, r => r.UserId, u => u.Id, (r, u) => new ReviewDetailsWithVote
                {
                    Review = r,
                    UserName = u.UserName,
                    IsDeleted = r.IsDeleted,
                    DislikesCount = r.ReviewsRatings.LongCount(rr => rr.ReactionType == -1),
                    LikesCount = r.ReviewsRatings.LongCount(rr => rr.ReactionType == 1),
                    UserVote = r.ReviewsRatings.Where(rr => rr.UserId == userId)
                        .Select(rr => rr.ReactionType == 1 ? (bool?)true : (rr.ReactionType == 0 ? null : (bool?)false))
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(token);
        }
    }
}
