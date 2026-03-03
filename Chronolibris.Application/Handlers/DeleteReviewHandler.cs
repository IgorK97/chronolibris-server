using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Unit>
    {
        private readonly IUnitOfWork _uow;

        public DeleteReviewCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Unit> Handle(DeleteReviewCommand cmd, CancellationToken ct)
        {
            var review = await _uow.Reviews.GetByIdAsync(cmd.ReviewId, ct)
                ?? throw new Exception("Review not found.");

            if (review.UserId != cmd.UserId)
                throw new Exception("You can only delete your own reviews.");

            if (review.DeletedAt != null)
                throw new Exception("Review is already deleted.");

            // Soft delete — Scenario 2: user can POST again to create a fresh review
            review.DeletedAt = DateTime.UtcNow;
            review.ReviewStatusId = 4;

            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
