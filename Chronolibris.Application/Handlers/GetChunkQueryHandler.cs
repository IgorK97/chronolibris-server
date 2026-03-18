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
                ?? throw new KeyNotFoundException($"BookFile {request.BookFileId} не найден");

            // Имя файла совпадает с тем, что пишет конвертер: 000.json, 001.json, … 
            //Что такое :D3???
            //var fileName = $"{request.ChunkIndex}.json";

            return await _storage.ReadChunkAsync(bookFile.Id.ToString(), request.ChunkIndex, "chunk", ct);
        }
    }
}
