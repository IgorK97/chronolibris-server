using MediatR;

namespace Chronolibris.Application.Requests.Bookmarks
{
    public record UpdateBookmarkCommand(long BookmarkId, long UserId, string? NoteText) : IRequest<bool>;

}
