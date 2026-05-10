using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.References
{
    public record GetRoleDetailsQuery(): IRequest<List<RoleDetails>>;
}
