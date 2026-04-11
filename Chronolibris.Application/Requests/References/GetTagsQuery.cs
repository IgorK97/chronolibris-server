using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.References
{
    public record GetTagsQuery(
        long? TagTypeId = null,
        string? SearchTerm = null,
        long LastId = 0,
        int Limit = 20
    ) : IRequest<PagedResult<TagDetails>>;
}
