using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Bookmarks;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Bookmarks
{
    public class GetBookmarksHandler(IBookmarkRepository bookmarkRepository) : IRequestHandler<GetBookmarksQuery, List<BookmarkDetails>>
    {

        public async Task<List<BookmarkDetails>> Handle(GetBookmarksQuery request, CancellationToken cancellationToken)
        {
            var bookmarks = await bookmarkRepository
                                            .GetAllForBookAndUserAsync(request.Bookid, request.UserId, cancellationToken);


            if (bookmarks == null)
            {
                return new List<BookmarkDetails>();
            }

            return bookmarks.Select(b => new BookmarkDetails
            {
                Id = b.Id,
                Note = b.Note,
                BookFileId=b.BookFileId,
                ParaIndex = b.ParaIndex,
                CreatedAt = b.CreatedAt
            }).ToList();
        }
    }
}
