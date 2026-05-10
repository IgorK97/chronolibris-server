using Chronolibris.Application.Requests.Books;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Interfaces.Services;
using MediatR;

namespace Chronolibris.Application.Handlers.Books
{
    public class GetBookFileHandler : IRequestHandler<GetBookFileQuery, Stream?>
    {
        private readonly IBookFileRepository _bookFileRepository;
        private readonly IStorageService _bookStorage;

        public GetBookFileHandler(IBookFileRepository bookFileRepository, IStorageService bookStorage)
        {
            _bookFileRepository = bookFileRepository;
            _bookStorage = bookStorage;
        }

        public async Task<Stream?> Handle(GetBookFileQuery request, CancellationToken cancellationToken)
        {
            var bookFile = await _bookFileRepository.GetByIdAsync(request.BookFileId, cancellationToken);
            if (bookFile == null || string.IsNullOrEmpty(bookFile.StorageUrl)) return null;
            string extension = ".fb2.zip";
            if (bookFile.FormatId == 2)
                extension = ".epub";
            return await _bookStorage.ReadBookSourceAsync(bookFile.Id.ToString(), extension, cancellationToken);
        }
    }

}
