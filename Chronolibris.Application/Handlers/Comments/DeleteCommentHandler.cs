using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.Comments
{
    public record DeleteCommentCommand(long CommentId, long UserId) : IRequest;
    public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand>
        {
            private readonly ICommentRepository _repository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCommentHandler(ICommentRepository repository, IUnitOfWork unitOfWork)
            {
                _repository = repository;
                _unitOfWork = unitOfWork;
            }

            public async Task Handle(DeleteCommentCommand request, CancellationToken ct)
            {
                var comment = await _repository.GetByIdAsync(request.CommentId, ct);

                if (comment == null || comment.UserId != request.UserId) return;

                comment.DeletedAt = DateTime.UtcNow;
                _repository.Update(comment);
                await _unitOfWork.SaveChangesAsync(ct);
            }
        }


       
    
}
