using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Bookmarks;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Bookmarks
{
    public class UpdateBookmarkHandler : IRequestHandler<UpdateBookmarkCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookmarkHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            UpdateBookmarkCommand request,
            CancellationToken cancellationToken)
        {
            var bookmark = await _unitOfWork.Bookmarks.GetByIdAsync(request.BookmarkId);
            //if(bookmark == null || bookmark.UserId != request.UserId)
            //{
            //    return false;
            //}
            if (bookmark == null)
                throw new ChronolibrisException("Такой закладки нет", ErrorType.NotFound);

            if (bookmark.UserId != request.UserId)
                throw new ChronolibrisException("Нет доступа", ErrorType.Forbidden);
            bookmark.Note = request.NoteText;
            //_unitOfWork.Bookmarks.Update(bookmark);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
