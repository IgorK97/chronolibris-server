using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References
{
    public class GetRoleDetailsHandler(IGenericRepository<PersonRole> personRolesRepository) : IRequestHandler<GetRoleDetailsQuery, List<RoleDetails>>
    {
        public async Task<List<RoleDetails>> Handle(GetRoleDetailsQuery query, CancellationToken token) {
            var result = await personRolesRepository.GetAllAsync(token);
            return result.Select(pr => new RoleDetails
            {
                Name = pr.Name,
                Id = pr.Id,
                Kind = (int)pr.Kind,
            }).ToList();
        }
    }
}
