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
                return;
            }

            if(existing.UserId != request.UserId)
            {
                throw new ChronolibrisException("Нет доступа на совершение этой операции", ErrorType.Forbidden); //или лучше вообще ничего не возвращать,
                //чтобы даже не поняли, есть ли чья-то такая закладка там или нет
            }

            _unitOfWork.Bookmarks.Delete(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);           
        }
    }
}
