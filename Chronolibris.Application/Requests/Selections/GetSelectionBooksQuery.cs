using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record GetSelectionBooksQuery(long SelectionId, long? LastId, int Limit, long userId, bool mode)
    : IRequest<PagedResult<BookListItem>>;

}
