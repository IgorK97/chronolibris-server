using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Requests.Search
{
    public record SimpleSearchKeysetQuery(
        string Query,
        int PageSize,
        long? UserId,
        double? LastBestSimilarity,
        long? LastId,
        bool mode
    ) : IRequest<PagedResult<BookSearchResult>>;
    public record AdvancedSearchKeysetQuery(
        string? Query,
        int PageSize,
        long? UserId,
        double? LastBestSimilarity,
        long? LastId,
        List<PersonRoleFilter> PersonFilters,
        long ThemeId,
        long SelectionId,
        List<long> RequiredTagIds,
        List<long> ExcludedTagIds,
        bool mode
    ) : IRequest<PagedResult<BookSearchResult>>;
}