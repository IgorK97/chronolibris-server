using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Shelves;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Shelves
{
    public class GetShelfBooksHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetShelfBooksQuery, PagedResult<BookListItem>>
    {
        public async Task<PagedResult<BookListItem>> Handle(GetShelfBooksQuery request, CancellationToken ct)
        {
            var shelf = await unitOfWork.Shelves.GetByIdAsync(request.ShelfId, ct);
            if (shelf is null || shelf.UserId != request.UserId)
            {
                throw new ChronolibrisException
                    ("Полка не найдена или доступ к ней ограничен", 
                    ErrorType.Forbidden);
            }
            long? lastId = request.LastId;
            var books = await unitOfWork.Shelves.GetBooksForShelfAsync(
                request.ShelfId, lastId, request.Limit + 1, request.UserId, ct);

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
                LastId = books.LastOrDefault()?.Id ?? lastId,
            };
        }
    }

}
