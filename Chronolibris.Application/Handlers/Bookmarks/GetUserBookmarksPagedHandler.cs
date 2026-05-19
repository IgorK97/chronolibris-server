using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Bookmarks;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Bookmarks
{
    public class GetUserBookmarksPagedHandler(IBookmarkRepository bookmarkRepository)
            : IRequestHandler<GetUserBookmarksPagedQuery, PagedResult<BookmarkWithBookDetails>>
    {
        public async Task<PagedResult<BookmarkWithBookDetails>> Handle(
            GetUserBookmarksPagedQuery request,
            CancellationToken cancellationToken)
        {
            var (items, totalCount) = await bookmarkRepository.GetPagedForUserAsync(
                request.UserId,
                request.Number,
                request.PageSize + 1,
                request.SearchQuery,
                cancellationToken);

            bool hasNext = items.Count > request.PageSize;

            if (hasNext)
            {
                items.RemoveAt(items.Count - 1);
            }

            return new PagedResult<BookmarkWithBookDetails>
            {
                Items = items.Select(b => new BookmarkWithBookDetails
                {
                    Id = b.Id,
                    Xpointer = b.Xpointer,
                    Context = b.Context,
                    Note = b.Note,
                    CreatedAt = b.CreatedAt,
                    BookFileId = b.BookFileId,
                    BookFileFormatName = b.BookFile?.Format?.Name ?? string.Empty,
                    BookFileFormatId = b.BookFile!.Format!.Id,
                    BookFileStatusId = b.BookFile!.StatusId,
                    BookId = b.BookFile?.BookId ?? 0,
                    BookTitle = b.BookFile?.Book?.Title ?? string.Empty,
                }).ToList(),
               Limit = request.PageSize,
               HasNext = hasNext,
               LastId = items.Last().Id,
            };
        }
    }
}
