using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record GetSelectionsRequest(
        int Page = 1,
        int PageSize = 20
    ) : IRequest<PagedResult<SelectionDetails>>;
}
