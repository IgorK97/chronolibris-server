using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Reviews;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Reviews
{
    public class CreateReviewHandler : IRequestHandler<CreateReviewCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateReviewHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<long> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {

            var existing = await _unitOfWork.Reviews.GetActiveByUserAndBookAsync(request.UserId, request.BookId, cancellationToken);
            if (existing != null)
                throw new ChronolibrisException("Пользователь уже оставил отзыв на эту книгу", ErrorType.Conflict);

            var book = await _unitOfWork.Books.GetByIdAsync(request.BookId, cancellationToken);
            if(book==null || !book.IsReviewable || !book.IsAvailable)
                throw new ChronolibrisException("Этой книги нет или она не может быть оценена", ErrorType.Unprocessable);

            var review = new Review
            {
                BookId = request.BookId,
                UserId = request.UserId,
                ReviewText = request.ReviewText ?? null,
                Score = request.Score,
                CreatedAt = DateTime.UtcNow,
                Id = 0,
                IsDeleted = false,
            };

            await _unitOfWork.Reviews.AddAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return review.Id;
        }
    }
}
