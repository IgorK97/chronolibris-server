using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    public record CreateCommentCommand(long BookId, long UserId, string Text, long? ParentCommentId) : IRequest<long>;

    public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, long>
    {
        private readonly IUnitOfWork _uow;
        public CreateCommentHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<long> Handle(CreateCommentCommand request, CancellationToken ct)
        {
            var comment = new Comment
            {
                BookId = request.BookId,
                UserId = request.UserId,
                Text = request.Text,
                ParentCommentId = request.ParentCommentId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
            };
            await _uow.Comments.AddAsync(comment, ct);
            await _uow.SaveChangesAsync();
            // Предполагается, что UnitOfWork сохранит изменения
            return comment.Id;
        }
    }
}
