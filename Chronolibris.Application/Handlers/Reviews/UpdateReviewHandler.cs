using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Reviews;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.Reviews
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Unit>
    {
        private readonly IUnitOfWork _uow;

        public UpdateReviewCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Unit> Handle(UpdateReviewCommand cmd, CancellationToken ct)
        {
            var review = await _uow.Reviews.GetByIdAsync(cmd.ReviewId, ct)
                ?? throw new Exception("Review not found.");

            if (review.UserId != cmd.UserId)
                throw new Exception("You can only edit your own reviews.");

            if (review.DeletedAt != null)
                throw new Exception("Cannot update a deleted review. Create a new one.");

            bool hadText = !string.IsNullOrWhiteSpace(review.ReviewText);
            bool hasText = !string.IsNullOrWhiteSpace(cmd.ReviewText);
            bool textAdded = !hadText && hasText; // Scenario 3: first added text to a score-only review

            review.Score = cmd.Score;
            review.ReviewText = cmd.ReviewText?.Trim();
            review.UpdatedAt = DateTime.UtcNow;

            //if (textAdded)
            //{
            //    // Scenario 3: adding text to a published score-only review → must go to moderation
            //    review.ReviewStatusId = 1;
            //}
            // Scenario 4: changing score on a review that already has text → status unchanged
            // (stays Published or Pending as it was — moderator's decision is respected)

            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
