using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Comments
{
    public record DeleteCommentCommand(long CommentId, long UserId) : IRequest;
    public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand>
        {
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCommentHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task Handle(DeleteCommentCommand request, CancellationToken ct)
            {
                var comment = await _unitOfWork.Comments.GetByIdAsync(request.CommentId, ct);

                if (comment == null || comment.UserId != request.UserId) return;

                comment.DeletedAt = DateTime.UtcNow;
                _unitOfWork.Comments.Update(comment);
                await _unitOfWork.SaveChangesAsync(ct);
            }
        }


       
    
}
