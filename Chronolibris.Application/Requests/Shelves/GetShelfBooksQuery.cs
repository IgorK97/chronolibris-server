using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Shelves
{
    public record GetShelfBooksQuery(long ShelfId, long? lastId, int Limit, long userId)
    : IRequest<PagedResult<BookListItem>>;

}
