using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Shelves;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Shelves
{
    public class GetShelfBooksHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetShelfBooksQuery, PagedResult<BookListItem>>
    {
        public async Task<PagedResult<BookListItem>> Handle(GetShelfBooksQuery request, CancellationToken ct)
        {

            long? lastId = request.lastId;


            var books = await unitOfWork.Shelves.GetBooksForShelfAsync(
                request.ShelfId, lastId, request.Limit, request.userId, ct);

            var hasNext = books.Count() > request.Limit;
            if (hasNext)
            {
                books.RemoveAt(books.Count() - 1);
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
