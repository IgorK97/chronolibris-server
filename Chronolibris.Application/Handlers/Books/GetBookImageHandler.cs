using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Books;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Interfaces.Services;
using MediatR;

namespace Chronolibris.Application.Handlers.Books
{
    public class GetBookImageHandler : IRequestHandler<GetBookImageQuery, Stream?>
    {
        private readonly IBookFileRepository bookFileRepository;
        private readonly IStorageService storageService;

        public GetBookImageHandler(IBookFileRepository bookFileRepository, IStorageService storageService)
        {
            this.bookFileRepository = bookFileRepository;
            this.storageService = storageService;
        }

        public async Task<Stream?> Handle(GetBookImageQuery request, CancellationToken ct)
        {
            //var bookFile = await bookFileRepository.GetByIdAsync(request.BookfileId, ct);
            //if (bookFile is null)
            //    return null;

            var bookFile = await bookFileRepository.GetByIdAsync(request.BookFileId, ct);
            if (bookFile == null) return null;
            if (!(bookFile.StatusId == BookFileStatuses.COMPLETED || bookFile.StatusId == BookFileStatuses.ARCHIVE) && request.Role == "reader") return null;
            if (request.Role == "reader" && bookFile.StatusId == BookFileStatuses.ARCHIVE)
            {
                bool res = await bookFileRepository.AnyAsync(b => b.Id == request.BookFileId && b.Bookmarks.Any(bm => bm.UserId == request.UserId), ct);
                if (!res) return null;
            }

            return await storageService.ReadImageAsync(request.BookFileId.ToString(), request.FileName, ct);
        }
    }
}
