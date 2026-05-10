using Chronolibris.Application.Models;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record GetSelectionsRequest(
        int Page = 1,
        int PageSize = 20
    ) : IRequest<PagedResult<SelectionDetails>>;
}
