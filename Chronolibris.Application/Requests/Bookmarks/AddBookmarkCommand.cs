using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Bookmarks
{
    public record AddBookmarkCommand(long bookFileId, long userId, string? noteText, int paraIndex) : IRequest<AddBookmarkResult>;

    public record AddBookmarkResult(long id, DateTime createdAt);
}
