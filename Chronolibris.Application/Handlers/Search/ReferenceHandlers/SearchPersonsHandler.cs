using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Handlers.Search.ReferenceHandlers
{
    public class SearchPersonsHandler
        : IRequestHandler<SearchPersonsQuery, List<PersonSuggestionDto>>
    {
        private readonly ISearchRepository _repo;
        public SearchPersonsHandler(ISearchRepository repo) => _repo = repo;
        public Task<List<PersonSuggestionDto>> Handle(
            SearchPersonsQuery request, CancellationToken ct)
            => _repo.SearchPersonsAsync(request.Name, request.Limit, ct);
    }
}
