using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models.Search;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    public class BookSearchRepository : IBookSearchRepository
    {
        private readonly ApplicationDbContext _context;
        public BookSearchRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Простой поиск (офсетный), similarity
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<OffsetPagedResult<BookSearchResult>> SearchOffsetAsync(
            SimpleSearchOffsetRequest request, CancellationToken token)
        {
            var query = request.Query.Trim();
            var skip = (request.Page - 1) * request.PageSize;
            var parameters = new List<object> { query, query, query };

            var sql = $"""
                SELECT sub.id, sub.best_similarity
                FROM (
                    SELECT
                        b.id,
                        GREATEST(
                            word_similarity($3::text, b.title),
                            COALESCE((
                                SELECT word_similarity($3::text, c.title)
                                FROM book_content bc
                                JOIN contents c ON c.id = bc.content_id
                                WHERE bc.book_id=b.id
                                ORDER BY 1 DESC
                                LIMIT 1
                            ), 0)
                        ) AS best_similarity
                    FROM books b
                    WHERE b.is_available = true
                    AND b.title %> $1::text

                    UNION

                    SELECT
                        b.id,
                        GREATEST(
                            word_similarity($3::text, b.title),
                            COALESCE((
                                SELECT word_similarity($3::text, c.title)
                                FROM book_content bc
                                JOIN contents c ON c.id=bc.content_id
                                WHERE bc.book_id=b.id
                                ORDER BY 1 DESC
                                LIMIT 1
                            ), 0)
                        ) AS best_similarity
                    FROM books b
                    WHERE b.is_available = true
                        AND EXISTS (
                            SELECT 1
                            FROM book_content bc
                            JOIN contents c ON c.id = bc.content_id
                            WHERE bc.book_id = b.id
                            AND c.title%>$2::text
                        )
                    ) sub
                    ORDER BY sub.best_similarity DESC, sub.id ASC
            """;

            var countSql = $"SELECT COUNT(*) FROM ({sql}) counted";
            var totalCount = await _context.Database.SqlQueryRaw<int>(countSql, parameters.ToArray()).SingleAsync(token);

            var pagedIds = await _context.Database.SqlQueryRaw<SearchIdScore>(
                sql + $"\nLIMIT {request.PageSize} OFFSET {skip}",
                parameters.ToArray())
                .ToListAsync(token);

            if (pagedIds.Count == 0)
                return new OffsetPagedResult<BookSearchResult>
                {
                    Items = [],
                    HasNext = false,
                    Page = request.Page,
                    TotalCount = totalCount,
                };
            var ids = pagedIds.Select(x=>x.Id).ToList();
            var itemsDict = await ProjectByIdsAsync(ids, request.UserId, token);
            var items = ids.Where(id => itemsDict.ContainsKey(id)).Select(id => itemsDict[id])
                .ToList();

            return new OffsetPagedResult<BookSearchResult>
            {
                Items = items,
                HasNext = skip + items.Count < totalCount,
                Page = request.Page,
                TotalCount = totalCount,
            };


        }
        /// <summary>
        /// Простой поиск (по ключу), similarity
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<PagedResult<BookSearchResult>>SearchKeysetAsync(
            SimpleSearchKeysetRequest request, CancellationToken token)
        {
            var query = request.Query.Trim();
            var parameters = new List<object> { query,request.PageSize+1 };

            var cursorClause = " ";
            if(request.LastBestSimilarity.HasValue && request.LastId.HasValue)
            {
                var p = parameters.Count;
                parameters.Add(request.LastBestSimilarity.Value);
                parameters.Add(request.LastId.Value);

                cursorClause = $"""
                    WHERE sub.best_similarity < ${p + 1} 
                    OR (sub.best_similarity = ${p + 1} AND sub.id > ${p + 2})
                    """;

            }

            var sql = @"
                SELECT sub.id, sub.best_similarity
                FROM (
                    SELECT
                        b.id,
                        GREATEST(
                            word_similarity({0}::text, b.title),
                            COALESCE((
                                SELECT word_similarity({0}::text, c.title)
                                FROM book_content bc
                                JOIN contents c ON c.id = bc.content_id
                                WHERE bc.book_id = b.id
                                ORDER BY 1 DESC
                                LIMIT 1
                            ), 0)
                        ) AS best_similarity
                    FROM books b
                    WHERE b.is_available = true
                        AND word_similarity({0}::text, b.title)>0.3

                    UNION

                    SELECT
                        b.id,
                        GREATEST(
                            word_similarity({0}::text, b.title),
                            COALESCE((
                                SELECT word_similarity({0}::text, c.title)
                                FROM book_content bc
                                JOIN contents c ON c.id = bc.content_id
                                WHERE bc.book_id = b.id
                                ORDER BY 1 DESC
                                LIMIT 1
                            ), 0)
                        ) AS best_similarity
                    FROM books b
                    WHERE b.is_available = true
                        AND EXISTS (
                            SELECT 1
                            FROM book_content bc
                            JOIN contents c ON c.id = bc.content_id
                            WHERE bc.book_id=b.id
                                AND word_similarity({0}::text, c.title) >0.3
                            )
                    ) sub
                    " + cursorClause+@"
                    ORDER BY sub.best_similarity DESC, sub.id ASC
                    LIMIT {1}
                ";

            var pagedIds = await _context.Database.SqlQueryRaw<SearchIdScore>(sql, parameters.ToArray())
                .ToListAsync(token);

            var hasNext = pagedIds.Count > request.PageSize;
            var pageItems = pagedIds.Take(request.PageSize).ToList();

            if (pageItems.Count == 0)
                return new PagedResult<BookSearchResult>
                {
                    Items = [],
                    HasNext = false,
                    LastId = null,
                    LastBestSimilarity = null,
                };

            var ids = pageItems.Select(x => x.Id).ToList();
            var itemsDict = await ProjectByIdsAsync(ids, request.UserId, token);

            var items = ids.Where(id => itemsDict.ContainsKey(id)).Select(ids => itemsDict[ids])
                .ToList();

            var last = pageItems.Last();
            return new PagedResult<BookSearchResult>
            {
                Items = items,
                HasNext = hasNext,
                LastId = last.Id,
                LastBestSimilarity = last.BestSimilarity,
            };

        }
        /// <summary>
        /// расширенный поиск (офсетная пагинация) + similarity
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<OffsetPagedResult<BookSearchResult>> AdvancedSearchOffsetAsync(
            AdvancedSearchOffsetRequest request, CancellationToken token)
        {
            var (sql, parameters) = BuildAdvancedUnionSql(request.Query, request);
            var skip = (request.Page - 1) * request.PageSize;

            var countSql = $"SELECT COUNT(*) FROM ({sql}) counted";
            var totalCount = await _context.Database.SqlQueryRaw<int>(countSql, parameters.ToArray())
                .SingleAsync(token);

            var pagedIds = await _context.Database.SqlQueryRaw<SearchIdScore>(
                sql + $"\nORDER BY best_similarity DESC, id ASC\n LIMIT {request.PageSize} OFFSET {skip}",
                parameters.ToArray())
                .ToListAsync(token);

            if (pagedIds.Count == 0)
                return new OffsetPagedResult<BookSearchResult>
                {
                    Items = [],
                    HasNext = false,
                    Page = request.Page,
                    TotalCount = totalCount,
                };

            var ids = pagedIds.Select(X500DistinguishedName => X500DistinguishedName.Id).ToList();
            var itemsDict = await ProjectByIdsAsync(ids, request.UserId, token);
            var items = ids.Where(ids => itemsDict.ContainsKey(ids))
                .Select(ids => itemsDict[ids]).ToList();

            return new OffsetPagedResult<BookSearchResult>
            {
                Items = items,
                HasNext = skip + items.Count < totalCount,
                Page = request.Page,
                TotalCount = totalCount,
            };
        }
        /// <summary>
        /// Расширенный поиск (пагинация по ключу) + similarity
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<PagedResult<BookSearchResult>> AdvancedSearchKeysetAsync(
            AdvancedSearchKeysetRequest request, CancellationToken token)
        {
            var (unionSql, parameters) = BuildAdvancedUnionSql(request.Query, request);

            var cursorClause = "";
            if(request.LastBestSimilarity.HasValue && request.LastId.HasValue)
            {
                var p = parameters.Count;
                parameters.Add(request.LastBestSimilarity.Value);
                parameters.Add(request.LastId.Value);

                cursorClause = $"""
                    WHERE best_similarity < ${p + 1}
                    OR (best_similarity =${p + 1} AND id>${p + 2})
                    """;

            }

            var sql = $"""
                SELECT id, best_similarity
                FROM ({unionSql}) sub
                {cursorClause}
                ORDER BY best_similarity DESC, id ASC
                LIMIT {request.PageSize + 1}
                """;

            var pagedIds = await _context.Database.SqlQueryRaw<SearchIdScore>(sql, parameters.ToArray())
                .ToListAsync(token);

            var hasNext = pagedIds.Count > request.PageSize;

            var pageItems = pagedIds.Take(request.PageSize).ToList();

            if (pageItems.Count == 0)
                return new PagedResult<BookSearchResult>
                {
                    Items = [],
                    HasNext = false,
                    LastId = null,
                    LastBestSimilarity = null,
                };

            var ids = pageItems.Select(X500DistinguishedName => X500DistinguishedName.Id).ToList();
            var itemsDict = await ProjectByIdsAsync(ids, request.UserId, token);
            var items = ids.Where(ids => itemsDict.ContainsKey(ids))
                .Select(ids => itemsDict[ids])
                .ToList();

            var last = pageItems.Last();
            return new PagedResult<BookSearchResult>
            {
                Items = items,
                HasNext = hasNext,
                LastId = last.Id,
                LastBestSimilarity = last.BestSimilarity,
            };
        }

        private static (string Sql, List<object> Parameters) BuildAdvancedUnionSql(
            string rawQuery,
            AdvancedSearchOffsetRequest? offsetRequest = null, AdvancedSearchKeysetRequest keysetRequest = null)
        {
            var publisherIds = offsetRequest?.PublisherIds ?? keysetRequest?.PublisherIds ?? [];
            var languageIds = offsetRequest?.LanguageIds ?? keysetRequest?.LanguageIds ?? [];
            var countryIds = offsetRequest?.CountryIds ?? keysetRequest?.CountryIds ?? [];
            var yearFrom = offsetRequest?.YearFrom ?? keysetRequest?.YearFrom;
            var yearTo = offsetRequest?.YearTo ?? keysetRequest?.YearTo;
            var personFilters = offsetRequest?.PersonFilters ?? keysetRequest?.PersonFilters ?? [];
            var requiredThemes = offsetRequest?.RequiredThemeIds ?? keysetRequest?.RequiredThemeIds ?? [];
            var excludedThemes = offsetRequest?.ExcludedThemeIds ?? keysetRequest?.ExcludedThemeIds ?? [];
            var requiredTags = offsetRequest?.RequiredTagIds ?? keysetRequest?.RequiredTagIds ?? [];
            var excludedTags = offsetRequest?.ExcludedTagIds ?? keysetRequest?.ExcludedTagIds ?? [];

            var query = rawQuery.Trim();
            var parameters = new List<object> { query, query, query };

            var bookFilters = new StringBuilder();

            if (publisherIds.Count > 0)
            {
                var p = parameters.Count + 1;
                parameters.Add(publisherIds.ToArray());
                bookFilters.AppendLine($"""AND b.publisher_id = ANY(${p})""");
            }

            if (languageIds.Count > 0)
            {
                var p = parameters.Count + 1;
                parameters.Add(languageIds.ToArray());
                bookFilters.AppendLine($"""AND b.language_id = ANY(${p})""");
            }

            if (countryIds.Count > 0)
            {
                var p = parameters.Count + 1;
                parameters.Add(countryIds.ToArray());
                bookFilters.AppendLine($"""AND b.country_id = ANY(${p})""");
            }

            if (yearFrom.HasValue)
            {
                var p = parameters.Count + 1;
                parameters.Add(yearFrom.Value);
                bookFilters.AppendLine($"""AND b.year >= ${p}""");
            }

            if (yearTo.HasValue)
            {
                var p = parameters.Count + 1;
                parameters.Add(yearTo.Value);
                bookFilters.AppendLine($"""AND b.year <= ${p}""");
            }
            foreach (var pf in personFilters)
            {
                var pRole = parameters.Count + 1;
                var pPersons = parameters.Count + 2;
                parameters.Add(pf.RoleId);
                parameters.Add(pf.PersonIds.ToArray());
                bookFilters.AppendLine($"""
                    AND (
                        EXISTS (
                            SELECT 1 FROM book_participations p
                            WHERE p.book_id = b.id
                              AND p.person_role_id = ${pRole}
                              AND p.person_id = ANY(${pPersons})
                        )
                        OR EXISTS (
                            SELECT 1
                            FROM book_content bc
                            JOIN content_participations p ON p.content_id = bc.content_id
                            WHERE bc.book_id = b.id
                              AND p.person_role_id = ${pRole}
                              AND p.person_id = ANY(${pPersons})
                        )
                    )
                    """);
            }

            if (requiredThemes.Count > 0)
            {
                var p = parameters.Count + 1;
                parameters.Add(requiredThemes.ToArray());
                bookFilters.AppendLine($"""
                    AND EXISTS (
                        SELECT 1
                        FROM book_content bc
                        JOIN content_theme ct ON ct.content_id = bc.content_id
                        WHERE bc.book_id = b.id
                          AND ct.theme_id = ANY(${p})
                    )
                    """);
            }

            if (excludedThemes.Count > 0)
            {
                var p = parameters.Count + 1;
                parameters.Add(excludedThemes.ToArray());
                bookFilters.AppendLine($"""
                    AND NOT EXISTS (
                        SELECT 1
                        FROM book_content bc
                        JOIN content_theme ct ON ct.content_id = bc.content_id
                        WHERE bc.book_id = b.id
                          AND ct.theme_id = ANY(${p})
                    )
                    """);
            }

            if (requiredTags.Count > 0)
            {
                var p = parameters.Count + 1;
                parameters.Add(requiredTags.ToArray());
                bookFilters.AppendLine($"""
                    AND EXISTS (
                        SELECT 1
                        FROM book_content bc
                        JOIN content_tags ctg ON ctg.content_id = bc.content_id
                        WHERE bc.book_id = b.id
                          AND ctg.tag_id = ANY(${p})
                    )
                    """);
            }

            if (excludedTags.Count > 0)
            {
                var p = parameters.Count + 1;
                parameters.Add(excludedTags.ToArray());
                bookFilters.AppendLine($"""
                    AND NOT EXISTS (
                        SELECT 1
                        FROM book_content bc
                        JOIN content_tags ctg ON ctg.content_id = bc.content_id
                        WHERE bc.book_id = b.id
                          AND ctg.tag_id = ANY(${p})
                    )
                    """);
            }
            var filters = bookFilters.ToString();

            // Подзапрос scoring — одинаков в обеих ветках
            var scoringSubquery = """
                GREATEST(
                    word_similarity($3::text, b.title),
                    COALESCE((
                        SELECT word_similarity($3::text, c.title)
                        FROM book_content bc
                        JOIN contents c ON c.id = bc.content_id
                        WHERE bc.book_id = b.id
                        ORDER BY 1 DESC
                        LIMIT 1
                    ), 0)
                )
                """;

            var sql = $"""
                SELECT b.id AS id, {scoringSubquery} AS best_similarity
                FROM books b
                WHERE b.is_available = true
                  AND b.title %> $1::text
                {filters}
 
                UNION
 
                SELECT b.id AS id, {scoringSubquery} AS best_similarity
                FROM books b
                WHERE b.is_available = true
                  AND EXISTS (
                      SELECT 1
                      FROM book_content bc
                      JOIN contents c ON c.id = bc.content_id
                      WHERE bc.book_id = b.id
                        AND c.title %> $2::text
                  )
                {filters}
                """;

            return (sql, parameters);


        }

        private static (string Sql, List<object> Parameters) BuildAdvancedUnionSql(
           string rawQuery,
           AdvancedSearchKeysetRequest request)
           => BuildAdvancedUnionSql(rawQuery, keysetRequest: request);
        private async Task<Dictionary<long, BookSearchResult>> ProjectByIdsAsync(
            List<long> ids,
            long? userId,
            CancellationToken cancellationToken)
        {
            var favoriteShelfType = 1;
            var publishedStatus = 2;
            return await _context.Books
                .AsNoTracking()
                .Where(b => ids.Contains(b.Id))
                .Select(b => new BookSearchResult
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    CoverPath = b.CoverPath,
                    Year = b.Year,
                    IsAvailable = b.IsAvailable,
                    IsReviewable = b.IsReviewable,
                    AverageRating = b.Reviews
                        .Where(r => !r.IsDeleted)
                        .Average(r => (double?)r.Score),
                    IsFavorite = userId != null &&
                        b.BookShelves.Any(bs =>
                            bs.Shelf.UserId == userId &&
                            bs.Shelf.ShelfTypeId == favoriteShelfType),
                })
                .ToDictionaryAsync(b => b.Id, cancellationToken);
        }

        private sealed class SearchIdScore
        {
            public long Id { get; set; }
            public double BestSimilarity { get; set; }
        }
    }
}
