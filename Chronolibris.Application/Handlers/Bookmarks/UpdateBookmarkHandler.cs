using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Bookmarks;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.Bookmarks
{
    public class UpdateBookmarkHandler : IRequestHandler<UpdateBookmarkCommand, bool>
    {
        private readonly IBookmarkRepository _bookmarkRepository; //Потом уточнить, будет ли работать,
                                                                  //если использовать и репозиторий,
                                                                  //и единицу работы одновременно параллельно
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookmarkHandler(IBookmarkRepository bookmarkRepository, IUnitOfWork unitOfWork)
        {
            _bookmarkRepository = bookmarkRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateBookmarkCommand request,
            CancellationToken cancellationToken)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(request.BookmarkId);
            if(bookmark == null || bookmark.UserId != request.UserId)
            {
                return false;
            }
            bookmark.Note = request.NoteText;
            await _unitOfWork.SaveChangesAsync();

            //_bookmarkRepository.Update(bookmark);

            return true;
        }
    }
}
