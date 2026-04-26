using MediatR;

namespace Chronolibris.Application.Requests.Bookmarks
{
    public record AddBookmarkCommand(long BookFileId, long UserId, string? NoteText, string Xpointer, string Context) : IRequest<AddBookmarkResult>;

    public record AddBookmarkResult(long Id, DateTime CreatedAt);
}
