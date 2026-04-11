using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Search
{
    public class SearchBooksHandler(IUnitOfWork unitOfWork) : IRequestHandler<SearchBooksQuery, PagedResult<BookListItem>>
    {
        public async Task<PagedResult<BookListItem>> Handle(SearchBooksQuery request, CancellationToken ct)
        {
            var books = await unitOfWork.Books.GetSearchedBooks(request.query, request.LastId, request.Limit, request.UserId, ct);
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
