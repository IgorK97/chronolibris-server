using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Interfaces;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Comments
{
    public record CreateCommentCommand(long BookId, long UserId, string Text, long? ParentCommentId) : IRequest<long>;

    public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, long>
    {
        private readonly IUnitOfWork _uow;
        private readonly IIdentityService _identityService;

        public CreateCommentHandler(IUnitOfWork uow, IIdentityService identityService)
        {
            _uow = uow;
            _identityService = identityService;
        }
        public async Task<long> Handle(CreateCommentCommand request, CancellationToken ct)
        {

            var book = await _uow.Books.GetByIdAsync(request.BookId);

            if (book == null || !book.IsAvailable)
                throw new ChronolibrisException("Книга отсутствует или недоступна", ErrorType.NotFound);

            bool userExists = await _identityService.IsUserActiveAsync(request.UserId);
            if (!userExists)
            {
                throw new ChronolibrisException("Нет доступа на совершение этой операции", ErrorType.Forbidden);
            }

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
