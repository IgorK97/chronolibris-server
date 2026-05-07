using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Handlers.Search
{
    public class SimpleSearchKeysetQueryHandler
        : IRequestHandler<SimpleSearchKeysetQuery, PagedBooks<BookSearchResult>>
    {
        private readonly ISearchRepository _searchRepository;

        public SimpleSearchKeysetQueryHandler(ISearchRepository searchRepository)
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
        : IRequestHandler<ComplexSearchQuery, PagedBooks<BookSearchResult>>
    {
        private readonly ISearchRepository _searchRepository;

        public AdvancedSearchKeysetQueryHandler(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public Task<PagedBooks<BookSearchResult>> Handle(
            ComplexSearchQuery request, CancellationToken cancellationToken)
        {
            return _searchRepository.ComplexSearchAsync(
               new ComplexSearchRequest
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