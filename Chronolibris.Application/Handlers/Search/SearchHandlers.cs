using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Handlers.Search
{
    public class SimpleSearchKeysetQueryHandler
        : IRequestHandler<SimpleSearchKeysetQuery, PagedBooks<BookSearchResult>>
    {
        private readonly IBookSearchRepository _searchRepository;

        public SimpleSearchKeysetQueryHandler(IBookSearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public Task<PagedBooks<BookSearchResult>> Handle(
            SimpleSearchKeysetQuery request, CancellationToken cancellationToken)
        {
            return _searchRepository.SearchKeysetAsync(
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

    public class AdvancedSearchKeysetQueryHandler
        : IRequestHandler<AdvancedSearchKeysetQuery, PagedBooks<BookSearchResult>>
    {
        private readonly IBookSearchRepository _searchRepository;

        public AdvancedSearchKeysetQueryHandler(IBookSearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public Task<PagedBooks<BookSearchResult>> Handle(
            AdvancedSearchKeysetQuery request, CancellationToken cancellationToken)
        {
            return _searchRepository.AdvancedSearchKeysetAsync(
               new AdvancedSearchKeysetRequest
               {
                   Query = request.Query,
                   PageSize = request.PageSize,
                   UserId = request.UserId,
                   LastBestSimilarity = request.LastBestSimilarity,
                   LastId = request.LastId,
                   PersonFilters = request.PersonFilters,
                   RequiredTagIds = request.RequiredTagIds,
                   ExcludedTagIds = request.ExcludedTagIds,
                   ThemeId = request.ThemeId,
                   SelectionId = request.SelectionId,
                   mode=request.Mode,
               },
                cancellationToken);
        }
    }
}