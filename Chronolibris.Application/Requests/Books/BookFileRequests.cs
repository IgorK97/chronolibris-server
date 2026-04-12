using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Books
{
    public record GetBookFilesQuery(long BookId) : IRequest<List<BookFileDto>>;

    public record GetBookFileDtoQuery(long BookFileId) : IRequest<BookFileDto?>;

    public record GetBookFileQuery(long BookFileId) : IRequest<Stream?>;
    public record UploadBookFileCommand(long BookId, int FormatId, bool IsReadable, Stream FileStream, string FileName, long FileSizeBytes,
        long CreatedBy) : IRequest<long>;
    

    public record DeleteBookFileCommand(long BookFileId, long UserId) : IRequest<Unit>;

    public record UpdateBookFileCommand(long BookId, int FormatId, bool IsReadable, Stream FileStream,
        string FileName, long FileSizeBytes, long UploadBy) : IRequest<long>;
    

    public record ProcessBookFileCommand(long BookFileId) : IRequest<Unit>;
}
