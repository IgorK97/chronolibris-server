using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Books
{
    public record GetBookFilesQuery(long BookId, bool adminMode) : IRequest<List<BookFileDto>>;
    public record GetBookFileQuery(long BookFileId, long UserId, string Role) : IRequest<Stream?>;
    public record UploadBookFileCommand(long BookId, int FormatId, bool IsReadable, 
        bool? HistoricalText, Stream FileStream, string FileName, long FileSizeBytes,
        long CreatedBy) : IRequest<long>;
    public record DeleteBookFileCommand(long BookFileId, long UserId) : IRequest<Unit>;

}
