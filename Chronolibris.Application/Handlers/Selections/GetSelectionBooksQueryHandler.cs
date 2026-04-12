using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Interfaces;
using MediatR;
using Chronolibris.Domain.Models;
using Chronolibris.Application.Requests.Selections;
using Chronolibris.Domain.Interfaces.Repository;

namespace Chronolibris.Application.Handlers.Selections
{
    public class GetSelectionBooksQueryHandler(ISelectionsRepository selectionsRepository)
    : IRequestHandler<GetSelectionBooksQuery, PagedResult<BookListItem>>
    {
        public async Task<PagedResult<BookListItem>> Handle(GetSelectionBooksQuery request, CancellationToken ct)
        {
            var books = await selectionsRepository
                .GetBooksForSelection(request.SelectionId, request.LastId, request.Limit, request.UserId, request.Mode, ct);

            bool hasNext = books.Count > request.Limit;

            if (hasNext)
            {
                books.RemoveAt(books.Count - 1);
            }

            return new PagedResult<BookListItem>
            {
                Items = books,
                Limit = request.Limit,
                HasNext = hasNext,
                LastId = books.LastOrDefault()?.Id 
            };
        }
    }

}
