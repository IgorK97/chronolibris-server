using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record GetSelectionsQuery(long? LastId = null, int Limit = 20, bool? OnlyActive=true) : IRequest<PagedResult<SelectionDetails>>;
    public record GetAllSelectionsQuery() : IRequest<IEnumerable<SelectionDetails>>;

}
