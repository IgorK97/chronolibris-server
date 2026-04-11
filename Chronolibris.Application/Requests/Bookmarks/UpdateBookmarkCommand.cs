using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.Bookmarks
{
    public record UpdateBookmarkCommand(long bookmarkId, long userId, string? noteText) : IRequest<bool>;

}
