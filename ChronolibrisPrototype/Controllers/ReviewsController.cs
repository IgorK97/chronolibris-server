using System.Security.Claims;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Application.Requests.Reviews;
using Chronolibris.Application.Requests.Users;
using Chronolibris.Infrastructure.Data;
using ChronolibrisPrototype.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetReviews(long bookId, long? lastId, int limit=20)
        {
            long? userId = null;
            if (!TryGetUserId(out var tryUserId))
                userId = null;
            else
            {
                userId = tryUserId;
            }

            if (limit < 1) limit = 20;
            else if (limit > 20) limit = 20;

            var reviews = await _mediator.Send(new GetReviewsQuery(bookId, lastId, limit, userId));
            return Ok(reviews);
        }

        [Authorize]
        [HttpGet("my/{bookId}")]
        public async Task<IActionResult> GetMyReview(long bookId)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var review = await _mediator.Send(new GetUserReviewForBookQuery(bookId, userId));

            //if (review == null) return NotFound();

            return Ok(review);
        }

        private bool TryGetUserId(out long userId)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(claim, out userId);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            var command = new CreateReviewCommand
            {
                BookId = request.BookId,
                UserId = userId,
                ReviewText = request.ReviewText,
                Score = request.Score
            };
            var reviewId = await _mediator.Send(command);
            return Ok(reviewId);
        }

        [Authorize]
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(long reviewId, UpdateReviewRequest request)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var command = new UpdateReviewCommand
            {
                ReviewId = reviewId,
                UserId = userId,
                ReviewText = request.ReviewText,
                Score = request.Score,
            };
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(long reviewId)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            await _mediator.Send(new DeleteReviewCommand { ReviewId = reviewId, UserId = userId });
            return NoContent();
        }

        [Authorize]
        [HttpPost("rate")]
        public async Task<IActionResult> RateReview(RateReviewRequest request)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var result = await _mediator.Send(
                new RateReviewCommand { 
                    ReviewId=request.ReviewId,
                    Score=request.Score,
                    UserId=userId
                });

            return Ok(result);
        }
    }
}
