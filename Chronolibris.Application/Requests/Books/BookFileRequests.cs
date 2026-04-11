using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Books
{
    public class GetBookFilesQuery : IRequest<List<BookFileDto>>
    {
        public long BookId { get; set; }
        public GetBookFilesQuery(long bookId) => BookId = bookId;
    }

    public class GetBookFileDtoQuery : IRequest<BookFileDto?>
    {
        public long BookFileId { get; set; }
        public GetBookFileDtoQuery(long bookFileId) => BookFileId = bookFileId;
    }

    public class GetBookFileQuery : IRequest<Stream?>
    {
        public long BookFileId { get; set; }
        public GetBookFileQuery(long bookFileId) => BookFileId = bookFileId;
    }
    public class UploadBookFileCommand : IRequest<long>
    {
        public long BookId { get; set; }
        public int FormatId { get; set; }
        public bool IsReadable { get; set; }
        public Stream FileStream { get; set; } = null!; 
        public string FileName { get; set; } = null!;
        public long FileSizeBytes { get; set; }
        public long CreatedBy { get; set; }

        public UploadBookFileCommand(
       long bookId, int formatId, bool isReadable,
       Stream fileStream, string fileName, long fileSizeBytes, long createdBy)
        {
            BookId = bookId;
            FormatId = formatId;
            IsReadable = isReadable;
            FileStream = fileStream;
            FileName = fileName;
            FileSizeBytes = fileSizeBytes;
            CreatedBy = createdBy;
        }
    }

    public class DeleteBookFileCommand : IRequest<Unit>
    {
        public long BookFileId { get; set; }
        public long UserId { get; set; }

        public DeleteBookFileCommand(long bookFileId, long userId)
        {
            BookFileId = bookFileId;
            UserId = userId;
        }
    }

    public class UpdateBookFileCommand : IRequest<long>
    {
        public long BookId { get; set; }
        public int FormatId { get; set; }
        public bool IsReadable { get; set; }
        public Stream FileStream { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public long FileSizeBytes { get; set; }
        public long UpdatedBy { get; set; }

        public UpdateBookFileCommand(
            long bookId, int formatId, bool isReadable,
            Stream fileStream, string fileName, long fileSizeBytes, long updatedBy)
        {
            BookId = bookId;
            FormatId = formatId;
            IsReadable = isReadable;
            FileStream = fileStream;
            FileName = fileName;
            FileSizeBytes = fileSizeBytes;
            UpdatedBy = updatedBy;
        }
    }

    public class ProcessBookFileCommand : IRequest<Unit>
    {
        public long BookFileId { get; set; }
        public ProcessBookFileCommand(long bookFileId) => BookFileId = bookFileId;
    }
}
