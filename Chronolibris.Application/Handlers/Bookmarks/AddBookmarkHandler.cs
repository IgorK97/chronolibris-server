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
            var availableBookFile = await _unitOfWork.BookFiles.GetByIdAsync(request.BookFileId);
            if(availableBookFile == null)
            {
                throw new ChronolibrisException("Такой книги нет или она недоступна", ErrorType.Forbidden);
            }

            bool userExists = await _identityService.IsUserActiveAsync(request.UserId);
            if (!userExists)
            {
                throw new ChronolibrisException("Нет доступа на совершение этой операции", ErrorType.Forbidden);
            }

            var oldBookmark = await _unitOfWork.Bookmarks.GetConcreteBookmark(request.BookFileId, request.UserId,
                request.ParaIndex, cancellationToken);

            if (oldBookmark != null)
                throw new ChronolibrisException("Закладка с такой позицией уже существует", ErrorType.Conflict);

            var bookmark = new Bookmark
            {
                BookFileId = request.BookFileId,
                UserId = request.UserId,
                Note = request.NoteText,
                ParaIndex = request.ParaIndex,
                CreatedAt = DateTime.UtcNow,
                Id = 0,
            };
            await _unitOfWork.Bookmarks.AddAsync(bookmark, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
           
            return new AddBookmarkResult(bookmark.Id, bookmark.CreatedAt);
        }
    }

}
