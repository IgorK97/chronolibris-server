using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Users;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.Reviews
{
    public class GetUserReviewForBookHandler(IReviewRepository reviewRepository) : IRequestHandler<GetUserReviewForBookQuery, MyReviewDetails?>
    {
        public async Task<MyReviewDetails?> Handle(GetUserReviewForBookQuery request, CancellationToken cancellationToken)
        {

            var review = await reviewRepository.GetActiveByUserAndBookAsync(request.UserId, request.BookId, cancellationToken);

            if(review is null)
            {
                return null;
            }
            return new MyReviewDetails
            {
                Id = review.Review.Id,
                Text = review.Review.ReviewText,
                Score = review.Review.Score,
                LikesCount = review.LikesCount,
                DislikesCount = review.DislikesCount,
                CreatedAt = review.Review.CreatedAt,
                UserVote = review.UserVote,
                //Status = review.IsD
                //Status = review.Review.ReviewStatus.Name,
                UserName = review.UserName,
            };

        }
    }
}
