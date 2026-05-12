using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Books;
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

        public Task<Stream?> Handle(GetBookImageQuery request, CancellationToken ct)
        {
            //var bookFile = await bookFileRepository.GetByIdAsync(request.BookfileId, ct);
            //if (bookFile is null)
            //    return null;

            return storageService.ReadImageAsync(request.BookFileId.ToString(), request.FileName, ct);
        }
    }
}
