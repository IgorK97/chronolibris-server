using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Search
{
    public record SearchBooksQuery(long UserId, long? LastId, int Limit, string query)
    : IRequest<PagedResult<BookListItem>>;
}
