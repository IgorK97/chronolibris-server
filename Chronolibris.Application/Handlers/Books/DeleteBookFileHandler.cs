using Chronolibris.Application.Requests.Books;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Interfaces.Services;
using MediatR;

namespace Chronolibris.Application.Handlers.Books
{
    public class DeleteBookFileHandler : IRequestHandler<DeleteBookFileCommand, Unit>
    {
        private readonly IStorageService _bookStorage;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBookFileHandler(
            IStorageService bookStorage,
            IUnitOfWork unitOfWork)
        {
            _bookStorage = bookStorage;
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(DeleteBookFileCommand request, CancellationToken cancellationToken)
        {
            var bookFile = await _unitOfWork.BookFiles.GetByIdAsync(request.BookFileId, cancellationToken);
            if (bookFile == null || bookFile.StatusId==BookFileStatuses.DELETED)
                return Unit.Value;

            if (bookFile.StatusId != BookFileStatuses.ARCHIVE && bookFile.StatusId!=BookFileStatuses.FAILED)
            {
                bookFile.StatusId = BookFileStatuses.ARCHIVE;
                bookFile.HiddenAt = DateTime.UtcNow;
                //bookFile.HiddenBy = request.UserId;
                _unitOfWork.BookFiles.Update(bookFile);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else
            {
                bookFile.StatusId=BookFileStatuses.DELETED;
                bookFile.DeletedAt = DateTime.UtcNow;
                //bookFile.DeletedBy = request.UserId;
                //_unitOfWork.BookFiles.Delete(bookFile);
                //await _unitOfWork.SaveChangesAsync(cancellationToken);
                _unitOfWork.BookFiles.Update(bookFile);
                var res = await _unitOfWork.SaveChangesAsync(cancellationToken);
                if(res>0) await _bookStorage.DeleteBookDataAsync(bookFile.Id.ToString(), cancellationToken);
            }

            return Unit.Value;
        }
    }
}
