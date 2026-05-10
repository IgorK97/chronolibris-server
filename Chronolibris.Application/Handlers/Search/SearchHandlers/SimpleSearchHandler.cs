using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Handlers.Search.SearchHandlers
{
    public class SimpleSearchHandler
           : IRequestHandler<SimpleSearchKeysetQuery, PagedBooks<BookSearchResult>>
    {
        private readonly ISearchRepository _searchRepository;

        public SimpleSearchHandler(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public Task<PagedBooks<BookSearchResult>> Handle(
            SimpleSearchKeysetQuery request, CancellationToken cancellationToken)
        {
            return _searchRepository.SimpleSearchAsync(
                new SimpleSearchKeysetRequest
                {
                    Query = request.Query,
                    PageSize = request.PageSize,
                    UserId = request.UserId,
                    LastBestSimilarity = request.LastBestSimilarity,
                    LastId = request.LastId,
                    mode = request.Mode
                },
                cancellationToken);
        }
    }
}
