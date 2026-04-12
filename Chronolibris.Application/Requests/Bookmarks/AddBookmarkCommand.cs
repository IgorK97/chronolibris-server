using MediatR;

namespace Chronolibris.Application.Requests.Bookmarks
{
    public record AddBookmarkCommand(long BookFileId, long UserId, string? NoteText, int ParaIndex) : IRequest<AddBookmarkResult>;

    public record AddBookmarkResult(long Id, DateTime CreatedAt);
}
