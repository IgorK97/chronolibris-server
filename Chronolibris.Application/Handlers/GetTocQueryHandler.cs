using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Interfaces.Services;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    public class GetTocQueryHandler : IRequestHandler<GetTocQuery, string?>
    {
        private readonly IStorageService _storage;
        private readonly IBookFileRepository _bookFiles;

        public GetTocQueryHandler(IStorageService storage, IBookFileRepository bookFiles)
        {
            _storage = storage;
            _bookFiles = bookFiles;
        }

        public async Task<string?> Handle(GetTocQuery request, CancellationToken ct)
        {
            var bookFile = await _bookFiles.GetByIdAsync(request.BookFileId, ct)
                ?? throw new KeyNotFoundException($"BookFile {request.BookFileId} не найден");

            return await _storage.ReadChunkAsync(bookFile.Id.ToString(), "toc.json", "toc", ct);
        }
    }
}
