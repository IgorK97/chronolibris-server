using System.Threading;
using Chronolibris.Application.Requests.Books;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Interfaces.Services;
using MediatR;

namespace Chronolibris.Application.Handlers.Books
{
    public class GetChunkQueryHandler : IRequestHandler<GetChunkQuery, string?>
    {
        private readonly IStorageService _storage;
        private readonly IBookFileRepository _bookFiles;

        public GetChunkQueryHandler(IStorageService storage, IBookFileRepository bookFiles)
        {
            _storage = storage;
            _bookFiles = bookFiles;
        }

        public async Task<string?> Handle(GetChunkQuery request, CancellationToken ct)
        {
            var bookFile = await _bookFiles.GetByIdAsync(request.BookFileId, ct)
                ?? throw new ChronolibrisException("Книга не найдена", ErrorType.NotFound);

            if (!(bookFile.StatusId == BookFileStatuses.COMPLETED || bookFile.StatusId == BookFileStatuses.ARCHIVE) && request.Role == "reader") return null;
            if (request.Role == "reader" && bookFile.StatusId == BookFileStatuses.ARCHIVE)
            {
                bool res = await _bookFiles.AnyAsync(b => b.Id == request.BookFileId && b.Bookmarks.Any(bm => bm.UserId == request.UserId), ct);
                if (!res) return null;
            }

            return await _storage.ReadChunkAsync(bookFile.Id.ToString(), request.ChunkIndex, false, ct);
        }
    }
}
