using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Search.Queries
{
    /// <summary>
    /// Простой поиск по названию. word_similarity, keyset-пагинация.
    /// Курсор: (LastBestSimilarity, LastId) — оба null на первой странице.
    /// </summary>
    public record SimpleSearchKeysetQuery(
        string Query,
        int PageSize,
        long? UserId,
        double? LastBestSimilarity,
        long? LastId
    ) : IRequest<PagedResult<BookSearchResult>>;

    /// <summary>
    /// Расширенный поиск с фильтрами. word_similarity, keyset-пагинация.
    /// Курсор: (LastBestSimilarity, LastId) — оба null на первой странице.
    /// </summary>
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
        List<long> ExcludedTagIds
    ) : IRequest<PagedResult<BookSearchResult>>;
}