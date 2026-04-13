using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;
using Chronolibris.Application.Requests.Reviews;
using Chronolibris.Domain.Interfaces.Repository;

namespace Chronolibris.Application.Handlers.Reviews
{
    public class GetReviewsHandler(IReviewRepository reviewRepository) : IRequestHandler<GetReviewsQuery, PagedResult<ReviewDetails>>
    {

        public async Task<PagedResult<ReviewDetails>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
        {

            var reviews = await reviewRepository.GetByBookIdAsync(request.BookId,
                request.LastId, request.Limit,request.UserId, cancellationToken);

            bool hasNext = reviews.Count() > request.Limit;
            if (hasNext)
            {
                reviews.RemoveAt(reviews.Count() - 1);
            }

            var rDtos = reviews
            .Select(r => new ReviewDetails
            {
                Id = r.Review.Id,
                //AverageRating = r.Review.AverageRating,
                Text = r.Review.ReviewText,
                DislikesCount = r.DislikesCount,
                LikesCount = r.LikesCount,
                //UserName = r.Review.Name,
                Score = r.Review.Score,
                //Title = r.Review.Title,
                CreatedAt = r.Review.CreatedAt,
                UserVote = r.UserVote,
                UserName=r.UserName,
            }).ToList();


            return new PagedResult<ReviewDetails>
            {
                Items = rDtos,
                Limit = request.Limit,
                HasNext = hasNext,
                LastId = reviews.LastOrDefault()?.Review.Id
            };
        }
    }

}
