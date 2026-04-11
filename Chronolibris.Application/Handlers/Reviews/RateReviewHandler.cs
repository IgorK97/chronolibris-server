using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Reviews;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.Reviews
{

    public class RateReviewHandler : IRequestHandler<RateReviewCommand, ReviewDetails?>
    {

        private readonly IUnitOfWork _unitOfWork;

        public RateReviewHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<ReviewDetails?> Handle(RateReviewCommand request, CancellationToken cancellationToken)
        {
            if (request.Score != 1 && request.Score != -1)
                throw new Exception("Неверная оценка");

            var review = await _unitOfWork.Reviews.GetByIdWithVotesAsync(request.ReviewId, request.UserId, cancellationToken);
            if (review == null)
                return null;

            //Потом разблокировать
            //if(review.Review.UserId == request.UserId) 
            //    return null;

            
            var rating = await _unitOfWork.ReviewReactions.GetReviewReactionByUserIdAsync(request.ReviewId,
                request.UserId, cancellationToken);

            if (rating is null)
            {
                rating = new ReviewReactions
                {
                    Id = 0,
                    ReviewId = request.ReviewId,
                    ReactionType = request.Score,
                    UserId = request.UserId,
                };
                await _unitOfWork.ReviewReactions.AddAsync(rating, cancellationToken);
            }
            else
            {
                rating.ReactionType = request.Score == rating.ReactionType ? (short)0 : request.Score;
            }




                await _unitOfWork.SaveChangesAsync(cancellationToken);

            //await _unitOfWork.Reviews.RecalculateRatingAsync(request.ReviewId, cancellationToken);


            _unitOfWork.Reviews.Detach(review.Review);
            var newReview = await _unitOfWork.Reviews.GetByIdAsync(request.ReviewId, cancellationToken);

            if (review == null) return null;


            return new ReviewDetails
            {
                Id = review.Review.Id,

                //AverageRating = review.AverageRating,
                DislikesCount = review.DislikesCount,
                LikesCount = review.LikesCount,


                CreatedAt = review.Review.CreatedAt,
                Score = review.Review.Score,
                Text = review.Review.ReviewText,
                UserName = review.UserName,
                //Title = review.Title,
                //UserName = review.Name,
                UserVote = request.Score switch
                {
                    1 => true,
                    -1 => false,
                    _ => null
                }
            };
        }



    }
}
