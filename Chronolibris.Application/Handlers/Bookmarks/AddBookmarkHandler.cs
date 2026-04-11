using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Bookmarks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.Bookmarks
{
    public class AddBookmarkHandler : IRequestHandler<AddBookmarkCommand, AddBookmarkResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddBookmarkHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AddBookmarkResult> Handle(AddBookmarkCommand request, CancellationToken cancellationToken)
        {
            //AAAAAAAAAAAAAAAA
            //TODO: FIX THIS!!! - нет проверки на то, что такая закладка уже есть (потом исправлю тип сущности и все будет работать)

            var bookmark = new Bookmark
            {
                BookFileId = request.bookFileId,
                UserId = request.userId,
                Note = request.noteText,
                ParaIndex = request.paraIndex,
                CreatedAt = DateTime.UtcNow,
                Id = 0,
            };
            await _unitOfWork.Bookmarks.AddAsync(bookmark, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
           
            return new AddBookmarkResult(bookmark.Id, bookmark.CreatedAt);
        }
    }

}
