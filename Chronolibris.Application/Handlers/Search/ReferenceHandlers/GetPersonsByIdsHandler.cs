using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Handlers.Search.ReferenceHandlers
{
    public class GetPersonsByIdsHandler : IRequestHandler<GetPersonsByIdsQuery,
        List<PersonSuggestionDto>>
    {
        private readonly ISearchRepository _repo;
        public GetPersonsByIdsHandler(ISearchRepository repository) => _repo = repository;

        public Task<List<PersonSuggestionDto>> Handle(
            GetPersonsByIdsQuery request, CancellationToken ct)
            => _repo.GetPersonsByIdsAsync(request.Ids, ct);
    }
}
