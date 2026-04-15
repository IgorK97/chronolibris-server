using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Requests.Reviews;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Reviews
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly IIdentityService _identityService;


        public UpdateReviewCommandHandler(IUnitOfWork uow, IIdentityService identityService)
        {
            _uow = uow;
            _identityService = identityService;
        }

        public async Task<Unit> Handle(UpdateReviewCommand cmd, CancellationToken ct)
        {
            var review = await _uow.Reviews.GetByIdAsync(cmd.ReviewId, ct);
            if (review == null || review.IsDeleted)
            {
                throw new ChronolibrisException("Отзыв не найден", ErrorType.NotFound);
            }

            bool userExists = await _identityService.IsUserActiveAsync(cmd.UserId);
            if (!userExists || review.UserId != cmd.UserId)
            {
                throw new ChronolibrisException("Нет доступа на совершение этой операции", ErrorType.Forbidden);
            }

            bool hadText = !string.IsNullOrWhiteSpace(review.ReviewText);
            bool hasText = !string.IsNullOrWhiteSpace(cmd.ReviewText);
            bool textAdded = !hadText && hasText;

            review.Score = cmd.Score;
            if(textAdded)
                review.ReviewText = cmd.ReviewText;
            review.UpdatedAt = DateTime.UtcNow;

            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
