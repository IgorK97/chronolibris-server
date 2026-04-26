using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Requests.Bookmarks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Bookmarks
{
    public class AddBookmarkHandler : IRequestHandler<AddBookmarkCommand, AddBookmarkResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        public AddBookmarkHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }
        public async Task<AddBookmarkResult> Handle(AddBookmarkCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            bool userExists = await _identityService.IsUserActiveAsync(request.UserId);
            if (!userExists)
            {
                throw new ChronolibrisException("Нет доступа на совершение этой операции", ErrorType.Forbidden);
            }
            var availableBookFile = await _unitOfWork.BookFiles.GetByIdAsync(request.BookFileId);
            if(availableBookFile == null || !availableBookFile.Book.IsAvailable)
            {
                throw new ChronolibrisException("Такой книги нет или она недоступна", ErrorType.Forbidden);
            }

            var currentCount = await _unitOfWork.Bookmarks.CountAsync(b => b.UserId == request.UserId && b.BookFileId == request.BookFileId, cancellationToken);
            if (currentCount >= 500)
            {
                throw new ChronolibrisException("Достигнут лимит закладок для этой книги", ErrorType.Validation);
            }

            var bookmark = new Bookmark
            {
                BookFileId = request.BookFileId,
                UserId = request.UserId,
                Note = request.NoteText,
                Xpointer = request.Xpointer,
                CreatedAt = DateTime.UtcNow,
                Context = request.Context,
                Id = 0,
            };
            await _unitOfWork.Bookmarks.AddAsync(bookmark, cancellationToken);

            //await _unitOfWork.SaveChangesAsync(cancellationToken); //потом подправлю
            await transaction.CommitAsync(cancellationToken);
            return new AddBookmarkResult(bookmark.Id, bookmark.CreatedAt);
        }
    }

}
