using Chronolibris.Application.Requests.Comments;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Comments
{
    public class RateCommentHandler : IRequestHandler<RateCommentCommand, CommentDto?>
    {

        private readonly IUnitOfWork _unitOfWork;

        public RateCommentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CommentDto?> Handle(RateCommentCommand request, CancellationToken cancellationToken)
        {
            if (request.Score != 1 && request.Score != -1)
                throw new ChronolibrisException("Неверная оценка", ErrorType.Validation);

            var comment = await _unitOfWork.Comments.GetByIdWithVotesAsync(request.CommentId, request.UserId, cancellationToken);
            if (comment == null || comment.IsDeleted)
                throw new ChronolibrisException("Комментарий не найден", ErrorType.NotFound);

            if (comment.UserId == request.UserId)
                throw new ChronolibrisException("Недоступно", ErrorType.Unprocessable);


            var rating = await _unitOfWork.CommentReactions.GetCommentReactionByUserIdAsync(request.CommentId,
                request.UserId, cancellationToken);

            if (rating is null)
            {
                rating = new CommentReactions
                {
                    Id = 0,
                    CommentId = request.CommentId,
                    ReactionType = request.Score,
                    UserId = request.UserId,
                };
                await _unitOfWork.CommentReactions.AddAsync(rating, cancellationToken);

            }
            else
            {
                rating.ReactionType = request.Score == rating.ReactionType ? (short)0 : request.Score; //интересно, как это делают в реальности,
                //но при необходимости можно просто физически удалять,
                //а если с проивзодительностью будут проблемы - то потом просто денормализовать и триггер добавить
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return comment;

        }
    }
}