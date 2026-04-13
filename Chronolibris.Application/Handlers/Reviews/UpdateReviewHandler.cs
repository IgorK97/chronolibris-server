using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Reviews;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Reviews
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Unit>
    {
        private readonly IUnitOfWork _uow;

        public UpdateReviewCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Unit> Handle(UpdateReviewCommand cmd, CancellationToken ct)
        {
            var review = await _uow.Reviews.GetByIdAsync(cmd.ReviewId, ct);
            if (review == null || !review.IsDeleted)
            {
                throw new ChronolibrisException("Отзыв не найден", ErrorType.NotFound);
            }

            if (review.UserId != cmd.UserId)
                throw new ChronolibrisException("Нет доступа", ErrorType.Forbidden);


            review.Score = cmd.Score;
            review.UpdatedAt = DateTime.UtcNow;

            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
