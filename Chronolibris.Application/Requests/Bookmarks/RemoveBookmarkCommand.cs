using MediatR;

namespace Chronolibris.Application.Requests.Bookmarks
{
    public record RemoveBookmarkCommand(long BookmarkId, long UserId):IRequest;
}
