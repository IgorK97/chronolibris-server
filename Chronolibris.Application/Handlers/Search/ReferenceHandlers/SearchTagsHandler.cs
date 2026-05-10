using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Handlers.Search.ReferenceHandlers
{
    public class SearchTagsHandler
        : IRequestHandler<SearchTagsQuery, List<TagSuggestionDto>>
    {
        private readonly ISearchRepository _repo;
        public SearchTagsHandler(ISearchRepository repo) => _repo = repo;
        public Task<List<TagSuggestionDto>> Handle(
            SearchTagsQuery request, CancellationToken ct)
            => _repo.SearchTagsAsync(request.Name, request.Limit, ct);
    }
}
