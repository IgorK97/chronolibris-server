using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using MediatR;

namespace Chronolibris.Application.Requests.Bookmarks
{
    public record GetBookmarksQuery(long Bookid, long UserId): IRequest<List<BookmarkDetails>>;

}
