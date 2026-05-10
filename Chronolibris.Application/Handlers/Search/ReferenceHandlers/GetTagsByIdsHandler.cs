using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Handlers.Search.ReferenceHandlers
{
    public class GetTagsByIdsHandler : IRequestHandler<GetTagsByIdsQuery, List<TagSuggestionDto>>
    {
        private readonly ISearchRepository _repo;
        public GetTagsByIdsHandler(ISearchRepository repo) => _repo = repo;

        public Task<List<TagSuggestionDto>> Handle(
            GetTagsByIdsQuery request, CancellationToken ct)
            => _repo.GetTagsByIdsAsync(request.Ids, ct);
    }
}
