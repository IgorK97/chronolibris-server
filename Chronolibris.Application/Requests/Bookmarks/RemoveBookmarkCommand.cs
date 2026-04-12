using MediatR;

namespace Chronolibris.Application.Requests.Bookmarks
{
    public record RemoveBookmarkCommand(long BbookmarkId, long UserId):IRequest<bool>;
}
