using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Books;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Books
{
    public class GetBookFilesHandler : IRequestHandler<GetBookFilesQuery, List<BookFileDto>>
    {
        private readonly IBookFileRepository _bookFileRepository;

        public GetBookFilesHandler(IBookFileRepository bookFileRepository)
        {
            _bookFileRepository = bookFileRepository;
        }

        public async Task<List<BookFileDto>> Handle(GetBookFilesQuery request, CancellationToken cancellationToken)
        {
            var bookFiles = await _bookFileRepository.GetByBookIdAsync(request.BookId, request.adminMode, cancellationToken);

            return bookFiles.Select(bf => new BookFileDto
            {
                Id = bf.Id,
                BookId = bf.BookId,
                FormatId = bf.FormatId,
                FormatName = bf.Format?.Name,
                StorageUrl = bf.StorageUrl,
                FileSizeBytes = bf.OriginalSize,
                StoredSizeBytes = bf.StoredSize,
                IsReadable = bf.IsReadable,
                CreatedAt = bf.CreatedAt,
                CompletedAt = bf.CompletedAt,
                //CreatedBy = bf.CreatedBy,
                //Version = bf.Version,
                BookFileStatusId = bf.StatusId,
                BookFileStatusName = bf.BookFileStatus?.Name,
                HistoricalText = bf.HistoricalText,
            }).ToList();
        }
    }

}
