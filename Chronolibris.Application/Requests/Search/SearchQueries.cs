using Chronolibris.Domain.Models;
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
        bool Mode
    ) : IRequest<PagedBooks<BookSearchResult>>;
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
        bool Mode
    ) : IRequest<PagedBooks<BookSearchResult>>;
}