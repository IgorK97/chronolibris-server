using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Reviews;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;
using static System.Collections.Specialized.BitVector32;

namespace Chronolibris.Application.Handlers.Reviews
{

    public class RateReviewHandler : IRequestHandler<RateReviewCommand, ReviewDetails?>
    {

        private readonly IUnitOfWork _unitOfWork;

        public RateReviewHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<ReviewDetails?> Handle(RateReviewCommand request, CancellationToken ct)
        {
            if (request.Score != 1 && request.Score != -1)
                throw new ChronolibrisException("Неверная оценка", ErrorType.Validation);

            var reviewDto = await _unitOfWork.Reviews.GetByIdWithVotesAsync(request.ReviewId, request.UserId, ct);
            if (reviewDto == null || reviewDto.IsDeleted)
                throw new ChronolibrisException("Отзыв не найден", ErrorType.NotFound);

            if (reviewDto.Review.UserId == request.UserId)
                throw new ChronolibrisException("Нельзя оценивать собственный отзыв", ErrorType.Forbidden);

            var rating = await _unitOfWork.ReviewReactions.GetReviewReactionByUserIdAsync(request.ReviewId,
                request.UserId, ct);

            if (rating is null)
            {
                rating = new ReviewReactions
                {
                    Id = 0,
                    ReviewId = request.ReviewId,
                    ReactionType = request.Score,
                    UserId = request.UserId,
                };
                await _unitOfWork.ReviewReactions.AddAsync(rating, ct);
            }
            else
            {
                rating.ReactionType = request.Score == rating.ReactionType ? (short)0 : request.Score;
            }

            await _unitOfWork.SaveChangesAsync(ct);
            //reviewDto = await _unitOfWork.Reviews.GetByIdWithVotesAsync(request.ReviewId, request.UserId, ct);
            //if(reviewDto is null) return null; // В принципе, логика не особо важная,
            //можно и не возвращать актуальное количество лайков или дизлайков.
            //Самое главное, что голос читателя был учтен

            return new ReviewDetails
            {
                Id = reviewDto.Review.Id,
                DislikesCount = reviewDto.DislikesCount,
                LikesCount = reviewDto.LikesCount,
                CreatedAt = reviewDto.Review.CreatedAt,
                Score = reviewDto.Review.Score,
                Text = reviewDto.Review.ReviewText,
                UserName = reviewDto.UserName,
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
