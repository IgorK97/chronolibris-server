using Chronolibris.Application.Search.Queries;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Search.Handlers
{
    public class SimpleSearchKeysetQueryHandler
        : IRequestHandler<SimpleSearchKeysetQuery, PagedResult<BookSearchResult>>
    {
        private readonly IBookSearchRepository _searchRepository;

        public SimpleSearchKeysetQueryHandler(IBookSearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public Task<PagedResult<BookSearchResult>> Handle(
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
                },
                cancellationToken);
        }
    }

    public class AdvancedSearchKeysetQueryHandler
        : IRequestHandler<AdvancedSearchKeysetQuery, PagedResult<BookSearchResult>>
    {
        private readonly IBookSearchRepository _searchRepository;

        public AdvancedSearchKeysetQueryHandler(IBookSearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public Task<PagedResult<BookSearchResult>> Handle(
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
               },
                cancellationToken);
        }
    }
}