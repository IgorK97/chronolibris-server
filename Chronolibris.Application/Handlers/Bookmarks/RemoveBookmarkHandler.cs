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

    public class RemoveBookmarkHandler : IRequestHandler<RemoveBookmarkCommand>
    {
        private readonly IUnitOfWork _unitOfWork;


        public RemoveBookmarkHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task Handle(RemoveBookmarkCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.Bookmarks.GetByIdAsync(request.BookmarkId, cancellationToken);
            if (existing == null)
            {
                return; //Уже удалена, могу указать на идемпотентность и пример с двумя окнами браузера
            }

            if(existing.UserId != request.UserId)
            {
                throw new ChronolibrisException("Нет доступа на совершение этой операции", ErrorType.Forbidden);
            }

            _unitOfWork.Bookmarks.Delete(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);           
        }
    }
}
