using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Bookmarks
{
    public record GetBookmarksQuery(long BookId, long UserId): IRequest<List<BookmarkDetails>>;

}
