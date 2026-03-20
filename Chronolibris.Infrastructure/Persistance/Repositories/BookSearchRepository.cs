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
            var parameters = new List<object> { query, query, query };

            var cursorClause = "";
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
                                WHERE bc.book_id = b.id
                                ORDER BY 1 DESC
                                LIMIT 1
                            ), 0)
                        ) AS best_similarity
                    FROM books b
                    WHERE b.is_available = true
                        AND b.title %>$1::text

                    UNION

                    SELECT
                        b.id,
                        GREATEST(
                            word_similarity($3::text, b.title),
                            COALESCE((
                                SELECT word_similarity($3::text, b.title)
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
                            FROM book_contents bc
                            JOIN contents c ON c.id = bc.content_id
                            WHERE bc.book_id=b.id
                                AND c.title %> $2::text
                            )
                    ) sub
                    {cursorClause}
                    ORDER BY sub.best_similarity DESC, sub.id ASC
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
        }












    //    public async Task<OffsetPagedResult<BookSearchResult>> AdvancedSearchOffsetAsync(AdvancedSearchOffsetRequest request, CancellationToken token)
    //    {
    //        var query = request.Query.Trim();
    //        var threshold = 0.3f;
    //        var booksQuery = _context.Books
    //            .AsNoTracking()
    //            .Where(b => b.IsAvailable &&
    //                (EF.Functions.TrigramsWordSimilarity(query, b.Title) > threshold ||
    //                 b.BookContents.Any(bc =>
    //                     EF.Functions.TrigramsWordSimilarity(query, bc.Content.Title) > threshold)));

    //        if (request.PublisherIds.Count > 0)
    //            booksQuery = booksQuery
    //                .Where(b => request.PublisherIds.Contains(b.PublisherId!.Value));

    //        if (request.LanguageIds.Count > 0)
    //            booksQuery = booksQuery
    //                .Where(b => request.LanguageIds.Contains(b.LanguageId));

    //        if (request.CountryIds.Count > 0)
    //            booksQuery = booksQuery
    //                .Where(b => request.CountryIds.Contains(b.CountryId));

    //        if (request.YearFrom.HasValue)
    //            booksQuery = booksQuery.Where(b => b.Year >= request.YearFrom);

    //        if (request.YearTo.HasValue)
    //            booksQuery = booksQuery.Where(b => b.Year <= request.YearTo);

    //        foreach (var personFilter in request.PersonFilters)
    //        {
    //            var roleId = personFilter.RoleId;
    //            var personIds = personFilter.PersonIds;

    //            booksQuery = booksQuery.Where(b =>
    //                b.Participations.Any(p =>
    //                    p.PersonRoleId == roleId &&
    //                    personIds.Contains(p.PersonId))
    //                ||
    //                b.BookContents.Any(bc =>
    //                    bc.Content.Participations.Any(p =>
    //                        p.PersonRoleId == roleId &&
    //                        personIds.Contains(p.PersonId))));
    //        }

    //        if (request.RequiredThemeIds.Count > 0)
    //        {
    //            var requiredThemes = request.RequiredThemeIds;
    //            booksQuery = booksQuery.Where(b =>
    //                b.BookContents.Any(bc =>
    //                    bc.Content.Themes.Any(t =>
    //                        requiredThemes.Contains(t.Id))));
    //        }

    //        if (request.ExcludedThemeIds.Count > 0)
    //        {
    //            var excludedThemes = request.ExcludedThemeIds;
    //            booksQuery = booksQuery.Where(b =>
    //                !b.BookContents.Any(bc =>
    //                    bc.Content.Themes.Any(t =>
    //                        excludedThemes.Contains(t.Id))));
    //        }
    //        if (request.RequiredTagIds.Count > 0)
    //        {
    //            var requiredTags = request.RequiredTagIds;
    //            booksQuery = booksQuery.Where(b =>
    //                b.BookContents.Any(bc =>
    //                    bc.Content.Tags.Any(t =>
    //                        requiredTags.Contains(t.Id))));
    //        }
    //        if (request.ExcludedTagIds.Count > 0)
    //        {
    //            var excludedTags = request.ExcludedTagIds;
    //            booksQuery = booksQuery.Where(b =>
    //                !b.BookContents.Any(bc =>
    //                    bc.Content.Tags.Any(t =>
    //                        excludedTags.Contains(t.Id))));
    //        }

    //        var orderedQuery = booksQuery
    //            .Select(b => new
    //            {
    //                Book = b,
    //                // GREATEST(sim_по_книге, MAX(sim_по_контентам))
    //                BestSimilarity = Math.Max(
    //                    (double)EF.Functions.TrigramsWordSimilarity(query, b.Title),
    //                    b.BookContents
    //                        .Select(bc => (double)EF.Functions.TrigramsWordSimilarity(query, bc.Content.Title))
    //                            .OrderByDescending(s => s)
    //                            .FirstOrDefault()
    //                    )
    //            })
    //            .OrderByDescending(x => x.BestSimilarity)
    //            //.OrderByDescending(b => EF.Functions.TrigramsWordSimilarity(query, b.Title))
    //            .ThenBy(b => b.Book.Id);

    //        var totalCount = await orderedQuery.CountAsync(token);

    //        var skip = (request.Page - 1) * request.PageSize;
    //        var pagedIds = await orderedQuery
    //            .Select(b => b.Book.Id)
    //            .Skip(skip)
    //            .Take(request.PageSize)
    //            .ToListAsync(token);

    //        if (pagedIds.Count == 0)
    //            return new OffsetPagedResult<BookSearchResult>
    //            {
    //                Items = [],
    //                HasNext = false,
    //                Page = request.Page,
    //                TotalCount = totalCount,
    //            };

    //        var itemsDict = await ProjectByIdsAsync(pagedIds, request.UserId, token);

    //        var items = pagedIds
    //            .Where(id => itemsDict.ContainsKey(id))
    //            .Select(id => itemsDict[id])
    //            .ToList();

    //        return new OffsetPagedResult<BookSearchResult>
    //        {
    //            Items = items,
    //            HasNext = skip + items.Count < totalCount,
    //            Page = request.Page,
    //            TotalCount = totalCount,
    //        };


    //    }

    //    public async Task<OffsetPagedResult<BookSearchResult>> SearchOffsetAsync(SimpleSearchOffsetRequest request, CancellationToken token)
    //    {
    //        var query = request.Query.Trim();
    //        float threshold = 0.3f;



    //        var booksQuery = _context.Books.AsNoTracking()
    //            .Where(b => b.IsAvailable &&
    //            (EF.Functions.TrigramsWordSimilarity(query, b.Title) > threshold ||
    //            b.BookContents.Any(bc =>
    //            EF.Functions.TrigramsWordSimilarity(query, bc.Content.Title) > threshold)));

    //        //var orderedQuery = booksQuery.OrderByDescending(b => EF.Functions.TrigramsWordSimilarity(query, b.Title))
    //        //    .ThenBy(b => b.Id);

    //        var orderedQuery = booksQuery
    //            .Select(b => new
    //            {
    //                Book = b,
    //                // GREATEST(sim_по_книге, MAX(sim_по_контентам)) — берём лучшее совпадение
    //                BestSimilarity = Math.Max(
    //                    (double)EF.Functions.TrigramsWordSimilarity(query, b.Title),
    //                    b.BookContents
    //                        .Select(bc => (double)EF.Functions.TrigramsWordSimilarity(query, bc.Content.Title))
    //                        .OrderByDescending(s => s)
    //                        .FirstOrDefault()
    //                )
    //            })
    //            .OrderByDescending(x => x.BestSimilarity)
    //            .ThenBy(x => x.Book.Id);

    //        var totalCount = await orderedQuery.CountAsync(token);

    //        var skip = (request.Page - 1) * request.PageSize;
    //        var pagedIds = await orderedQuery.Select(b => b.Book.Id)
    //            .Skip(skip)
    //            .Take(request.PageSize)
    //            .ToListAsync(token);

    //        if (pagedIds.Count == 0)
    //        {
    //            return new OffsetPagedResult<BookSearchResult>
    //            {
    //                Items = [],
    //                HasNext = false,
    //                Page = request.Page,
    //                TotalCount = totalCount,
    //            };
    //        }
    //        var itemsDict = await ProjectByIdsAsync(pagedIds, request.UserId, token);

    //        var items = pagedIds.Where(id => itemsDict.ContainsKey(id))
    //            .Select(id => itemsDict[id])
    //            .ToList();

    //        return new OffsetPagedResult<BookSearchResult>
    //        {
    //            Items = items,
    //            HasNext = skip + items.Count < totalCount,
    //            Page = request.Page,
    //            TotalCount = totalCount,
    //        };

    //    }

    //    private async Task<Dictionary<long, BookSearchResult>> ProjectByIdsAsync(
    //        List<long> ids,
    //        long? userId,
    //        CancellationToken cancellationToken)
    //    {
    //        var favoriteShelfType = 1;
    //        var publishedStatus = 2;
    //        return await _context.Books
    //            .AsNoTracking()
    //            .Where(b => ids.Contains(b.Id))
    //            .Select(b => new BookSearchResult
    //            {
    //                Id = b.Id,
    //                Title = b.Title,
    //                Description = b.Description,
    //                CoverPath = b.CoverPath,
    //                Year = b.Year,
    //                IsAvailable = b.IsAvailable,
    //                IsReviewable = b.IsReviewable,
    //                AverageRating = b.Reviews
    //                    .Where(r => r.ReviewStatusId == publishedStatus)
    //                    .Average(r => (double?)r.Score),
    //                IsFavorite = userId != null &&
    //                    b.BookShelves.Any(bs =>
    //                        bs.Shelf.UserId == userId &&
    //                        bs.Shelf.ShelfTypeId == favoriteShelfType),
    //            })
    //            .ToDictionaryAsync(b => b.Id, cancellationToken);
    //    }



    //    public async Task<PagedResult<BookSearchResult>> SearchKeysetAsync(
    //SimpleSearchKeysetRequest request,
    //CancellationToken token)
    //    {
    //        var query = request.Query.Trim();
    //        var threshold = 0.3f;

    //        var booksQuery = _context.Books
    //            .AsNoTracking()
    //            .Where(b => b.IsAvailable &&
    //                (EF.Functions.TrigramsWordSimilarity(query, b.Title) > threshold ||
    //                 b.BookContents.Any(bc =>
    //                     EF.Functions.TrigramsWordSimilarity(query, bc.Content.Title) > threshold)));

    //        var projectedQuery = booksQuery
    //            .Select(b => new
    //            {
    //                Book = b,
    //                BestSimilarity = Math.Max(
    //                    (double)EF.Functions.TrigramsWordSimilarity(query, b.Title),
    //                    b.BookContents
    //                        .Select(bc => (double)EF.Functions.TrigramsWordSimilarity(query, bc.Content.Title))
    //                        .OrderByDescending(s => s)
    //                        .FirstOrDefault()
    //                )
    //            });

    //        if (request.LastBestSimilarity.HasValue && request.LastId.HasValue)
    //        {
    //            var lastSim = request.LastBestSimilarity.Value;
    //            var lastId = request.LastId.Value;
    //            projectedQuery = projectedQuery
    //                .Where(x => x.BestSimilarity < lastSim ||
    //                            (x.BestSimilarity == lastSim && x.Book.Id > lastId));
    //        }

    //        var pagedIds = await projectedQuery
    //            .OrderByDescending(x => x.BestSimilarity)
    //            .ThenBy(x => x.Book.Id)
    //            .Select(x => new { x.Book.Id, x.BestSimilarity })
    //            .Take(request.PageSize + 1)
    //            .ToListAsync(token);

    //        var hasNext = pagedIds.Count > request.PageSize;
    //        var pageItems = pagedIds.Take(request.PageSize).ToList();

    //        if (pageItems.Count == 0)
    //            return new PagedResult<BookSearchResult>
    //            {
    //                Items = [],
    //                HasNext = false,
    //                LastId = null,
    //                LastBestSimilarity = null,
    //            };

    //        var ids = pageItems.Select(x => x.Id).ToList();
    //        var itemsDict = await ProjectByIdsAsync(ids, request.UserId, token);

    //        var items = ids
    //            .Where(id => itemsDict.ContainsKey(id))
    //            .Select(id => itemsDict[id])
    //            .ToList();

    //        var last = pageItems.Last();
    //        return new PagedResult<BookSearchResult>
    //        {
    //            Items = items,
    //            HasNext = hasNext,
    //            LastId = last.Id,
    //            LastBestSimilarity = last.BestSimilarity,
    //        };
    //    }

    //    public async Task<PagedResult<BookSearchResult>> AdvancedSearchKeysetAsync(
    //        AdvancedSearchKeysetRequest request,
    //        CancellationToken token)
    //    {
    //        var query = request.Query.Trim();
    //        var threshold = 0.3f;

    //        var booksQuery = _context.Books
    //            .AsNoTracking()
    //            .Where(b => b.IsAvailable &&
    //                (EF.Functions.TrigramsWordSimilarity(query, b.Title) > threshold ||
    //                 b.BookContents.Any(bc =>
    //                     EF.Functions.TrigramsWordSimilarity(query, bc.Content.Title) > threshold)));

    //        if (request.PublisherIds.Count > 0)
    //            booksQuery = booksQuery
    //                .Where(b => request.PublisherIds.Contains(b.PublisherId!.Value));

    //        if (request.LanguageIds.Count > 0)
    //            booksQuery = booksQuery
    //                .Where(b => request.LanguageIds.Contains(b.LanguageId));

    //        if (request.CountryIds.Count > 0)
    //            booksQuery = booksQuery
    //                .Where(b => request.CountryIds.Contains(b.CountryId));

    //        if (request.YearFrom.HasValue)
    //            booksQuery = booksQuery.Where(b => b.Year >= request.YearFrom);

    //        if (request.YearTo.HasValue)
    //            booksQuery = booksQuery.Where(b => b.Year <= request.YearTo);

    //        foreach (var personFilter in request.PersonFilters)
    //        {
    //            var roleId = personFilter.RoleId;
    //            var personIds = personFilter.PersonIds;

    //            booksQuery = booksQuery.Where(b =>
    //                b.Participations.Any(p =>
    //                    p.PersonRoleId == roleId &&
    //                    personIds.Contains(p.PersonId))
    //                ||
    //                b.BookContents.Any(bc =>
    //                    bc.Content.Participations.Any(p =>
    //                        p.PersonRoleId == roleId &&
    //                        personIds.Contains(p.PersonId))));
    //        }

    //        if (request.RequiredThemeIds.Count > 0)
    //        {
    //            var requiredThemes = request.RequiredThemeIds;
    //            booksQuery = booksQuery.Where(b =>
    //                b.BookContents.Any(bc =>
    //                    bc.Content.Themes.Any(t =>
    //                        requiredThemes.Contains(t.Id))));
    //        }

    //        if (request.ExcludedThemeIds.Count > 0)
    //        {
    //            var excludedThemes = request.ExcludedThemeIds;
    //            booksQuery = booksQuery.Where(b =>
    //                !b.BookContents.Any(bc =>
    //                    bc.Content.Themes.Any(t =>
    //                        excludedThemes.Contains(t.Id))));
    //        }

    //        if (request.RequiredTagIds.Count > 0)
    //        {
    //            var requiredTags = request.RequiredTagIds;
    //            booksQuery = booksQuery.Where(b =>
    //                b.BookContents.Any(bc =>
    //                    bc.Content.Tags.Any(t =>
    //                        requiredTags.Contains(t.Id))));
    //        }

    //        if (request.ExcludedTagIds.Count > 0)
    //        {
    //            var excludedTags = request.ExcludedTagIds;
    //            booksQuery = booksQuery.Where(b =>
    //                !b.BookContents.Any(bc =>
    //                    bc.Content.Tags.Any(t =>
    //                        excludedTags.Contains(t.Id))));
    //        }

    //        var projectedQuery = booksQuery
    //            .Select(b => new
    //            {
    //                Book = b,
    //                BestSimilarity = Math.Max(
    //                    (double)EF.Functions.TrigramsWordSimilarity(query, b.Title),
    //                    b.BookContents
    //                        .Select(bc => (double)EF.Functions.TrigramsWordSimilarity(query, bc.Content.Title))
    //                        .OrderByDescending(s => s)
    //                        .FirstOrDefault()
    //                )
    //            });

    //        if (request.LastBestSimilarity.HasValue && request.LastId.HasValue)
    //        {
    //            var lastSim = request.LastBestSimilarity.Value;
    //            var lastId = request.LastId.Value;
    //            projectedQuery = projectedQuery
    //                .Where(x => x.BestSimilarity < lastSim ||
    //                            (x.BestSimilarity == lastSim && x.Book.Id > lastId));
    //        }

    //        var pagedIds = await projectedQuery
    //            .OrderByDescending(x => x.BestSimilarity)
    //            .ThenBy(x => x.Book.Id)
    //            .Select(x => new { x.Book.Id, x.BestSimilarity })
    //            .Take(request.PageSize + 1)
    //            .ToListAsync(token);

    //        var hasNext = pagedIds.Count > request.PageSize;
    //        var pageItems = pagedIds.Take(request.PageSize).ToList();

    //        if (pageItems.Count == 0)
    //            return new PagedResult<BookSearchResult>
    //            {
    //                Items = [],
    //                HasNext = false,
    //                LastId = null,
    //                LastBestSimilarity = null,
    //            };

    //        var ids = pageItems.Select(x => x.Id).ToList();
    //        var itemsDict = await ProjectByIdsAsync(ids, request.UserId, token);

    //        var items = ids
    //            .Where(id => itemsDict.ContainsKey(id))
    //            .Select(id => itemsDict[id])
    //            .ToList();

    //        var last = pageItems.Last();
    //        return new PagedResult<BookSearchResult>
    //        {
    //            Items = items,
    //            HasNext = hasNext,
    //            LastId = last.Id,
    //            LastBestSimilarity = last.BestSimilarity,
    //        };
    //    }
    }
}
