using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chronolibris.Application.Search.Queries;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models.Search;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    public class SearchRepository : IBookSearchRepository
    {
        private readonly ApplicationDbContext _context;
        public SearchRepository(ApplicationDbContext context)
        {
            _context = context;
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

                cursorClause = @"
                    WHERE sub.best_similarity < {" + p.ToString() +@"} 
                    OR (sub.best_similarity = {" + p.ToString() + @"} AND sub.id > {" + p.ToString() + @"})
                    ";

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
                    WHERE " + (!request.mode ?  @" b.is_available = true 
                        AND " : " ") + @" word_similarity({0}::text, b.title)>0.3

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
                    WHERE " + (!request.mode ?  @" b.is_available = true 
                        AND " : " ") +  @" EXISTS (
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
        /// Расширенный поиск (пагинация по ключу) + similarity
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<PagedResult<BookSearchResult>> AdvancedSearchKeysetAsync(
            AdvancedSearchKeysetRequest request, CancellationToken token)
        {
            var (stringSQL, parameters) = BuildString(request.Query, request);

            var cursorClause = "";
            if(request.LastBestSimilarity.HasValue && request.LastId.HasValue)
            {
                var p = parameters.Count;
                parameters.Add(request.LastBestSimilarity.Value);
                parameters.Add(request.LastId.Value);

                cursorClause = $$""" 
                    WHERE best_similarity < {{{p}}}
                    OR (best_similarity ={{{p}}} AND id>{{{p + 1}}}) 
                    """;

            }

            var sql = $""" 
                SELECT id, best_similarity
                FROM ({stringSQL}) sub
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

            var ids = pageItems.Select(x => x.Id).ToList();
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

        private static (string Sql, List<object> Parameters) BuildString(
            string? rawQuery,AdvancedSearchKeysetRequest request)
        {

            var query = rawQuery?.Trim() ?? string.Empty;
            var isStringSearch = !string.IsNullOrEmpty(query);
            var parameters = new List<object> { query };

            var bookFilters = new StringBuilder();

            foreach (var pf in request.PersonFilters)
            {
                var pRole = parameters.Count;
                var pPersons = parameters.Count + 1;
                parameters.Add(pf.RoleId);
                parameters.Add(pf.PersonIds.ToArray());
                bookFilters.AppendLine($$""" 
                    AND (
                        EXISTS (
                            SELECT 1 FROM book_participations p
                            WHERE p.book_id = b.id
                              AND p.person_role_id = {{{pRole}}}
                              AND p.person_id = ANY({{{pPersons}}})
                        )
                        OR EXISTS (
                            SELECT 1
                            FROM book_content bc
                            JOIN content_participations p ON p.content_id = bc.content_id
                            WHERE bc.book_id = b.id
                              AND p.person_role_id = {{{pRole}}}
                              AND p.person_id = ANY({{{pPersons}}})
                        )
                    ) 
                    """);
            }

            if (request.ThemeId > 0)
            {
                var p = parameters.Count;
                parameters.Add(request.ThemeId);
                bookFilters.AppendLine($$""" 
                    AND EXISTS (
                        SELECT 1
                        FROM book_content bc
                        JOIN content_theme ct ON ct.content_id = bc.content_id
                        WHERE bc.book_id = b.id
                          AND ct.theme_id = {{{p}}}
                    ) 
                    """);
            } 

            if(request.SelectionId > 0)
            {
                var p = parameters.Count;
                parameters.Add(request.SelectionId);
                bookFilters.AppendLine($$""" 
                    AND EXISTS (
                        SELECT 1
                        FROM book_selection bs
                        --FROM book_content bc
                        --JOIN books b ON bc.content_id = b.id
                        --JOIN book_selection bs ON b.id = bs.book_id
                        WHERE bs.selection_id = {{{p}}} AND bs.book_id = b.id
                    ) 
                    """);
            }

            if (request.RequiredTagIds.Count > 0)
            {
                var p = parameters.Count;
                parameters.Add(request.RequiredTagIds.ToArray());
                bookFilters.AppendLine($$""" 
                    AND EXISTS (
                        SELECT 1
                        FROM book_content bc
                        JOIN content_tags ctg ON ctg.contents_id = bc.content_id
                        WHERE bc.book_id = b.id
                          AND ctg.tags_id = ANY({{{p}}})
                    ) 
                    """);
            }

            if (request.ExcludedTagIds.Count > 0)
            {
                var p = parameters.Count;
                parameters.Add(request.ExcludedTagIds.ToArray());
                bookFilters.AppendLine($$""" 
                    AND NOT EXISTS (
                        SELECT 1
                        FROM book_content bc
                        JOIN content_tags ctg ON ctg.contents_id = bc.content_id
                        WHERE bc.book_id = b.id
                          AND ctg.tags_id = ANY({{{p}}})
                    ) 
                    """);
            }
            var filters = bookFilters.ToString();

            if (isStringSearch)
            {
                var scoringSubquery = $$""" 
                GREATEST(
                    word_similarity({{{0}}}::text, b.title),
                    COALESCE((
                        SELECT word_similarity({{{0}}}::text, c.title)
                        FROM book_content bc
                        JOIN contents c ON c.id = bc.content_id
                        WHERE bc.book_id = b.id
                        ORDER BY 1 DESC
                        LIMIT 1
                    ), 0)
                ) 
                """;

                var sql = $$""" 
                SELECT b.id AS id, {{scoringSubquery}} AS best_similarity
                FROM books b
                WHERE  
                """ + (!request.mode ? $$""" b.is_available = true AND """ : " ")  + 
                $$"""
                  word_similarity({{{0}}}::text, b.title)>0.3
                
                {{filters}}
 
                UNION
 
                SELECT b.id AS id, {{scoringSubquery}} AS best_similarity
                FROM books b
                WHERE  
                """ + (!request.mode ? $$""" b.is_available = true AND """ : " ")  +
                 $$"""
                  
                 EXISTS (
                 
                      SELECT 1
                      FROM book_content bc
                      JOIN contents c ON c.id = bc.content_id
                      WHERE bc.book_id = b.id
                        AND word_similarity({{{0}}}::text, c.title)>0.3
                  )
                {{filters}}
                """;

                return (sql, parameters);
            }
            else
            {
                var sql = $"""
                    SELECT b.id AS id, 1.0 AS best_similarity
                    FROM books b 
                    """ + (!request.mode ? $$""" WHERE b.is_available = true """ : string.IsNullOrEmpty(filters)? " " : " WHERE ") +
                    $"""
                    {filters}
                    
                    """;
                return (sql, parameters);
            }

        }

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
