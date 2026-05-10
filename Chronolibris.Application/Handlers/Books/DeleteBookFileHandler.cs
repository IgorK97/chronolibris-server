using Chronolibris.Application.Requests.Books;
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
            if (bookFile == null)
                return Unit.Value;

            _unitOfWork.BookFiles.Delete(bookFile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _bookStorage.DeleteBookDataAsync(bookFile.Id.ToString(), cancellationToken);

            return Unit.Value;
        }
    }
}
