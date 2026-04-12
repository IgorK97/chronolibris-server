using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Bookmarks
{
    public record GetBookmarksQuery(long Bookid, long UserId): IRequest<List<BookmarkDetails>>;

}
