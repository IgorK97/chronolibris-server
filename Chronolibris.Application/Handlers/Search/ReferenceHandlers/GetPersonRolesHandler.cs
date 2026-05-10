using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Handlers.Search.ReferenceHandlers
{
    public class GetPersonRolesHandler : IRequestHandler<GetPersonRolesQuery, List<PersonRoleDto>>
    {
        private readonly ISearchRepository _repo;
        public GetPersonRolesHandler(ISearchRepository repo) => _repo = repo;
        public Task<List<PersonRoleDto>> Handle(GetPersonRolesQuery _, CancellationToken ct)
            => _repo.GetAllPersonRolesAsync(ct);
    }
}
