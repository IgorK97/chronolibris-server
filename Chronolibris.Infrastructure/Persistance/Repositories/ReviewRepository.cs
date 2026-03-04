using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.Persistance.Repositories
{
    /// <summary>
    /// Репозиторий для сущности <see cref="Review"/>.
    /// </summary>
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ReviewRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения.</param>
        public ReviewRepository(ApplicationDbContext context) : base(context) { }

        public async Task<ReviewDetailsWithVote?> GetActiveByUserAndBookAsync(long userId, long bookId, CancellationToken token = default)
        {
            return await _context.Reviews.AsNoTracking()
                .Where(r => r.UserId == userId && r.BookId == bookId && r.ReviewStatusId != 4)
                .Select(r => new ReviewDetailsWithVote
                {
                    Review = r,
                    DislikesCount = r.ReviewsRatings.LongCount(rr => rr.ReactionType == -1),
                    LikesCount = r.ReviewsRatings.LongCount(rr => rr.ReactionType == 1),
                    UserVote = r.ReviewsRatings.Where(rr => rr.UserId == userId)
                        .Select(rr => (bool?)(rr.ReactionType == 1))
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(token);
        }


        //public async Task<long> CountLikesForReview(long reviewId, CancellationToken cancellationToken)
        //{
        //    return await _context.ReviewsRatings.LongCountAsync(rr => rr.ReviewId == reviewId && rr.Score == 1, cancellationToken);
        //}


        //public async Task<long> CountDislikesForReview(long reviewId, CancellationToken token)
        //{
        //    return await _context.ReviewsRatings.LongCountAsync(rr => rr.ReviewId == reviewId && rr.Score == -1, token);
        //}

        //public async Task<long> GetAverageForReview(long reviewId, CancellationToken token)
        //{
        //    return await _context.ReviewsRatings.Where(rr => rr.ReviewId == reviewId).SumAsync(rr => (long)rr.Score, token);
        //}

        /// <summary>
        /// Асинхронно получает все отзывы для указанной книги.
        /// </summary>
        /// <param name="bookId">Идентификатор книги.</param>
        /// <param name="token">Токен отмены.</param>
        /// <returns>Коллекция отзывов.</returns>
        public async Task<List<ReviewDetailsWithVote>> GetByBookIdAsync(long bookId, long? lastId, int limit, long? userId, CancellationToken token)
        {

            var query = _context.Reviews.AsNoTracking()
                .Where(r => r.BookId == bookId);

            if (lastId.HasValue)
            {
                query = query.Where(r => r.Id > lastId.Value);
            }

            return await query.Where(r => r.ReviewStatusId == 2 && r.ReviewText!= null).OrderBy(r => r.Id).Take(limit+1)
                .Select(r => new ReviewDetailsWithVote
                {
                    Review = r,
                    DislikesCount = r.ReviewsRatings.LongCount(rr => rr.ReactionType == -1),
                    LikesCount = r.ReviewsRatings.LongCount(rr => rr.ReactionType == 1),
                    UserVote = userId ==null? null
                    : r.ReviewsRatings.Where(rr=>rr.UserId==userId.Value)
                    .Select(rr=>(bool?)(rr.ReactionType == 1)).FirstOrDefault()
                }).ToListAsync(token);

            //var limitedReviews = await query
            //    .OrderBy(r => r.Id)
            //    .Take(limit + 1)
            //    .ToListAsync(token);

            //if (!limitedReviews.Any())
            //{
            //    return new List<ReviewDetailsWithVote>();
            //}

            //var reviewIds = limitedReviews.Select(r => r.Id).ToList();

            //var results = limitedReviews.AsQueryable()
            //    .Select(r => new ReviewDetailsWithVote 
            //    {
            //        Review = r,
            //        UserVote = _context.ReviewsRatings
            //            .Where(rr => rr.ReviewId == r.Id && rr.UserId == userId)
            //            .Select(rr => (bool?)(rr.ReactionType == 1))
            //            .FirstOrDefault()
            //    })
            //    .ToList();

            //return results;

            //return await query.OrderBy(r => r.Id)
            //    .Take(limit + 1)
            //    .ToListAsync(token);
        }

        public async Task<ReviewDetailsWithVote?> GetByIdWithVotesAsync(long reviewId, long userId, CancellationToken token = default)
        {
            return await _context.Reviews.AsNoTracking()
                .Where(r => r.Id == reviewId)
                .Select(r => new ReviewDetailsWithVote
                {
                    Review = r,
                    DislikesCount = r.ReviewsRatings.LongCount(rr => rr.ReactionType == -1),
                    LikesCount = r.ReviewsRatings.LongCount(rr => rr.ReactionType == 1),
                    UserVote = r.ReviewsRatings.Where(rr => rr.UserId == userId)
                        .Select(rr => (bool?)(rr.ReactionType == 1))
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(token);
        }

        /// <summary>
        /// Атомарно пересчитывает счетчики лайков, дизлайков и средний рейтинг для отзыва, 
        /// обновляя сущность <see cref="Review"/> в базе данных.
        /// </summary>
        /// <remarks>
        /// Использует <c>ExecuteUpdateAsync</c> для выполнения обновления непосредственно на уровне SQL. 
        /// Это обеспечивает атомарность и предотвращает проблемы конкурентности (Lost Update).
        /// </remarks>
        /// <param name="reviewId">Идентификатор отзыва для пересчета.</param>
        /// <param name="token">Токен отмены.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        //public async Task RecalculateRatingAsync(long reviewId, CancellationToken token)
        //{
        //    var likesQuery = _context.ReviewsRatings
        //        .Where(rr => rr.ReviewId == reviewId && rr.ReactionType == 1)
        //        .LongCount();

        //    var dislikesQuery = _context.ReviewsRatings
        //        .Where(rr => rr.ReviewId == reviewId && rr.ReactionType == -1)
        //        .LongCount();

        //    var averageQuery = _context.ReviewsRatings
        //        .Where(rr => rr.ReviewId == reviewId)
        //        .Sum(rr => (long?)rr.ReactionType) ?? 0;

        //    //await _context.Reviews
        //    //    .Where(r => r.Id == reviewId)
        //    //        .ExecuteUpdateAsync(setter => setter
        //    //            .SetProperty(r => r.LikesCount, (long) likesQuery)
        //    //            .SetProperty(r => r.DislikesCount, (long) dislikesQuery)
        //    //            .SetProperty(r => r.AverageRating, averageQuery),
        //    //            token);
        //}
    }
}
