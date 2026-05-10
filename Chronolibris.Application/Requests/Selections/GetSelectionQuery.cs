using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record GetSelectionQuery(long SelectionId, long UserId, string UserRole) : IRequest<SelectionDetails?>;
}
