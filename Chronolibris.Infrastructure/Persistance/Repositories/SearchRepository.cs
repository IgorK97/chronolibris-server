using System.Text;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using Chronolibris.Domain.Models.Search;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private readonly ApplicationDbContext _context;
        private const int SynonymRelationTypeId = 1;

        public SearchRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PagedBooks<BookSearchResult>>SimpleSearchAsync(
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
                    WHERE (sub.best_similarity < {" + p +@"} 
                    OR (sub.best_similarity = {" + p + @"} AND sub.id > {" + (p+1).ToString() + @"})) 
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
                        AND " : " ") + @" b.title%>{0}::text

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
                        AND " : " ") + @" EXISTS (
                            SELECT 1
                            FROM book_content bc
                            JOIN contents c ON c.id = bc.content_id
                            WHERE bc.book_id=b.id
                                AND c.title%>{0}::text
                            )
                    ) sub
                    " + cursorClause+@" 
                    ORDER BY sub.best_similarity DESC, sub.id ASC
                    LIMIT {1}
                ";

            var pagedIds = await _context.Database.SqlQueryRaw<SearchIdScore>(sql, parameters.ToArray())
                .ToListAsync(token);

            return await BuildPageAsync(pagedIds, request.PageSize,
                    request.UserId,
                    token);

            //var hasNext = pagedIds.Count > request.PageSize;
            //var pageItems = pagedIds.Take(request.PageSize).ToList();

            //if (pageItems.Count == 0)
            //    return new PagedBooks<BookSearchResult>
            //    {
            //        Items = [],
            //        HasNext = false,
            //        LastId = null,
            //        LastBestSimilarity = null,
            //    };

            //var ids = pageItems.Select(x => x.Id).ToList();
            //var itemsDict = await ProjectByIdsAsync(ids, request.UserId, token);

            //var items = ids.Where(id => itemsDict.ContainsKey(id)).Select(ids => itemsDict[ids])
            //    .ToList();

            //var last = pageItems.Last();
            //return new PagedBooks<BookSearchResult>
            //{
            //    Items = items,
            //    HasNext = hasNext,
            //    LastId = last.Id,
            //    LastBestSimilarity = last.BestSimilarity,
            //};

        }

        public async Task<PagedBooks<BookSearchResult>> ComplexSearchAsync(
            ComplexSearchRequest request, CancellationToken token)
        {
            var (stringSQL, parameters) = BuildString(request);

            var cursorClause = "";
            if(request.LastBestSimilarity.HasValue && request.LastId.HasValue)
            {
                var p = parameters.Count;
                parameters.Add(request.LastBestSimilarity.Value);
                parameters.Add(request.LastId.Value);

                cursorClause = $$""" 
                    WHERE (best_similarity < {{{p}}}
                    OR (best_similarity ={{{p}}} AND id>{{{p + 1}}})) 
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

            return await BuildPageAsync(pagedIds, request.PageSize,
                request.UserId,
                token);
            //var hasNext = pagedIds.Count > request.PageSize;

            //var pageItems = pagedIds.Take(request.PageSize).ToList();

            //if (pageItems.Count == 0)
            //    return new PagedBooks<BookSearchResult>
            //    {
            //        Items = [],
            //        HasNext = false,
            //        LastId = null,
            //        LastBestSimilarity = null,
            //    };

            //var ids = pageItems.Select(x => x.Id).ToList();
            //var itemsDict = await ProjectByIdsAsync(ids, request.UserId, token);
            //var items = ids.Where(ids => itemsDict.ContainsKey(ids))
            //    .Select(ids => itemsDict[ids])
            //    .ToList();

            //var last = pageItems.Last();
            //return new PagedBooks<BookSearchResult>
            //{
            //    Items = items,
            //    HasNext = hasNext,
            //    LastId = last.Id,
            //    LastBestSimilarity = last.BestSimilarity,
            //};
        }

        private static (string Sql, List<object> Parameters) BuildString(
            ComplexSearchRequest request) //порядок аргументов не важен?
        {

            var query = request.Query?.Trim() ?? string.Empty;
            var isStringSearch = !string.IsNullOrEmpty(query);
            var parameters = new List<object> { query };

            //var bookFilters = new StringBuilder();
            var bookFilters = BuildCommonFilters(request, parameters);
            var availFilter = !request.mode ? "b.is_available = true AND" : "";


            //foreach (var pf in request.PersonFilters)
            //{
            //    var pRole = parameters.Count;
            //    var pPersons = parameters.Count + 1;
            //    parameters.Add(pf.RoleId);
            //    parameters.Add(pf.PersonIds.ToArray());
            //    bookFilters.AppendLine($$""" 
            //        AND (
            //            EXISTS (
            //                SELECT 1 FROM book_participations p
            //                WHERE p.book_id = b.id
            //                  AND p.person_role_id = {{{pRole}}}
            //                  AND p.person_id = ANY({{{pPersons}}})
            //            )
            //            OR EXISTS (
            //                SELECT 1
            //                FROM book_content bc
            //                JOIN content_participations p ON p.content_id = bc.content_id
            //                WHERE bc.book_id = b.id
            //                  AND p.person_role_id = {{{pRole}}}
            //                  AND p.person_id = ANY({{{pPersons}}})
            //            )
            //        ) 
            //        """);
            //}

            //if (request.ThemeId > 0)
            //{
            //    var p = parameters.Count;
            //    parameters.Add(request.ThemeId);
            //    //bookFilters.AppendLine($$""" 
            //    //    AND EXISTS (
            //    //        SELECT 1
            //    //        FROM book_content bc
            //    //        JOIN content_theme ct ON ct.content_id = bc.content_id
            //    //        WHERE bc.book_id = b.id
            //    //          AND ct.theme_id = {{{p}}}
            //    //    ) 
            //    //    """);

            //    bookFilters.AppendLine($$"""
            //        AND EXISTS (
            //           WITH RECURSIVE expanded_themes AS (
            //              SELECT id FROM themes WHERE id = {{{p}}}
            //              UNION
            //              SELECT themes.id FROM themes
            //              INNER JOIN expanded_themes ethe ON themes.parent_theme_id = ethe.id
            //            )
            //        SELECT 1
            //        FROM book_content bc
            //        JOIN content_theme cthe ON cthe.content_id = bc.content_id
            //        WHERE bc.book_id = b.id
            //           AND cthe.theme_id IN (SELECT id FROM expanded_themes)
            //        )
            //        """);
            //}

            //if (request.SelectionId > 0)
            //{
            //    var p = parameters.Count;
            //    parameters.Add(request.SelectionId);
            //    bookFilters.AppendLine($$""" 
            //        AND EXISTS (
            //            SELECT 1
            //            FROM book_selection bs
            //            --FROM book_content bc
            //            --JOIN books b ON bc.content_id = b.id
            //            --JOIN book_selection bs ON b.id = bs.book_id
            //            WHERE bs.selection_id = {{{p}}} AND bs.book_id = b.id
            //        ) 
            //        """);
            //}

            //if (request.RequiredTagIds.Count > 0)
            //{
            //    foreach (var tagId in request.RequiredTagIds)
            //    {
            //        var p = parameters.Count;
            //        parameters.Add(tagId);

            //        bookFilters.AppendLine($$"""
            //            AND EXISTS (
            //               WITH RECURSIVE expanded_tags AS (
            //                  SELECT id FROM tags WHERE id = {{{p}}}
            //                  UNION
            //                  SELECT t.id FROM tags t
            //                  INNER JOIN expanded_tags et ON t.parent_tag_id = et.id
            //               )
            //               SELECT 1
            //               FROM book_content bc
            //               JOIN content_tags ctg ON ctg.contents_id = bc.content_id
            //               WHERE bc.book_id = b.id
            //                  AND ctg.tags_id IN (SELECT id FROM expanded_tags)
            //            )
            //            """);
            //    }



            //    //    //var p = parameters.Count;
            //    //    //parameters.Add(request.RequiredTagIds.ToArray());
            //    //    //bookFilters.AppendLine($$""" 
            //    //    //    AND EXISTS (
            //    //    //        SELECT 1
            //    //    //        FROM book_content bc
            //    //    //        JOIN content_tags ctg ON ctg.contents_id = bc.content_id
            //    //    //        WHERE bc.book_id = b.id
            //    //    //          AND ctg.tags_id = ANY({{{p}}})
            //    //    //    ) 
            //    //    //    """);
            //}

            //if (request.ExcludedTagIds.Count > 0)
            //{
            //    var p = parameters.Count;
            //    parameters.Add(request.ExcludedTagIds.ToArray());

            //    bookFilters.AppendLine($$"""
            //        AND NOT EXISTS (
            //           WITH RECURSIVE expanded_excluded_tags AS (
            //              SELECT id FROM tags WHERE id = ANY({{{p}}})
            //              UNION
            //              SELECT t.id FROM tags t
            //              INNER JOIN expanded_excluded_tags eet ON t.parent_tag_id = eet.id
            //           )
            //           SELECT 1
            //           FROM book_content bc
            //           JOIN content_tags ctg ON ctg.contents_id = bc.content_id
            //           WHERE bc.book_id = b.id
            //              AND ctg.tags_id IN (SELECT id FROM expanded_excluded_tags)
            //        )
            //        """);

            //    //bookFilters.AppendLine($$""" 
            //    //    AND NOT EXISTS (
            //    //        SELECT 1
            //    //        FROM book_content bc
            //    //        JOIN content_tags ctg ON ctg.contents_id = bc.content_id
            //    //        WHERE bc.book_id = b.id
            //    //          AND ctg.tags_id = ANY({{{p}}})
            //    //    ) 
            //    //    """);
            //}
            //var filters = bookFilters.ToString();

            if (isStringSearch)
            {
                return BuildStringScoredSql(bookFilters.ToString(), availFilter, parameters);
                //var scoringSubquery = $$""" 
                //GREATEST(
                //    word_similarity({{{0}}}::text, b.title),
                //    COALESCE((
                //        SELECT word_similarity({{{0}}}::text, c.title)
                //        FROM book_content bc
                //        JOIN contents c ON c.id = bc.content_id
                //        WHERE bc.book_id = b.id
                //        ORDER BY 1 DESC
                //        LIMIT 1
                //    ), 0)
                //) 
                //""";

                //var sql = $$""" 
                //SELECT b.id AS id, {{scoringSubquery}} AS best_similarity
                //FROM books b
                //WHERE  
                //""" + (!request.mode ? $$""" b.is_available = true AND """ : " ")  + 
                //$$"""
                //  b.title%>{{{0}}}::text
                
                //{{filters}}
 
                //UNION
 
                //SELECT b.id AS id, {{scoringSubquery}} AS best_similarity
                //FROM books b
                //WHERE  
                //""" + (!request.mode ? $$""" b.is_available = true AND """ : " ")  +
                // $$"""
                  
                // EXISTS (
                 
                //      SELECT 1
                //      FROM book_content bc
                //      JOIN contents c ON c.id = bc.content_id
                //      WHERE bc.book_id = b.id
                //        AND c.title%>{{{0}}}::text
                //  )
                //{{filters}}
                //""";

                //return (sql, parameters);
            }
            else
            {
                return BuildTagScoredSql(request.RequiredTagIds, bookFilters.ToString(), availFilter, parameters);
                //var sql = $"""
                //    SELECT b.id AS id, 1.0 AS best_similarity
                //    FROM books b 
                //    """ + (!request.mode ? $$""" WHERE b.is_available = true """ : string.IsNullOrEmpty(filters)? " " : " WHERE 1=1 ") + //потом посмотреть, как подправить
                //    $"""
                //    {filters}
                    
                //    """;
                //return (sql, parameters);
            }

        }

        private static (string Sql, List<object> Parameters) BuildStringScoredSql(
            string filters, string availFilter, List<object> parameters)
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
                WHERE {{availFilter}} 
                  b.title%>{{{0}}}::text
                
                {{filters}}
 
                UNION
 
                SELECT b.id AS id, {{scoringSubquery}} AS best_similarity
                FROM books b
                WHERE  {{availFilter}}  
                EXISTS (
                 
                      SELECT 1
                      FROM book_content bc
                      JOIN contents c ON c.id = bc.content_id
                      WHERE bc.book_id = b.id
                        AND c.title%>{{{0}}}::text
                  )
                {{filters}}
                """;

            return (sql, parameters);
        }

        private static (string Sql, List<object> Parameters) BuildTagScoredSql(
            IReadOnlyList<long> requiredTagIds,
            string filters,
            string availFilter,
            List<object> parameters)
        {
            if(requiredTagIds.Count == 0)
            { //добавить бы потом враиативность для фильтров, по типу проверки первый фильтр или нет
                var plainSql = $"""
                    SELECT b.id AS id, 1.0 AS best_similarity
                    FROM books b
                    {(!string.IsNullOrWhiteSpace(availFilter) || !string.IsNullOrWhiteSpace(filters) ?
                    $"where {availFilter} 1=1" : "")}
                    {filters}
                    """;
                return (plainSql, parameters);
            }

            var depthSubqueries = new List<string>();
            foreach(var tagId in requiredTagIds)
            {
                var p = parameters.Count;
                parameters.Add(tagId);

                depthSubqueries.Add($$"""
                    COALESCE((
                       WITH RECURSIVE tag_descendants AS (
                          SELECT id, 0 AS depth
                          FROM tags
                          WHERE id = {{{p}}}

                          UNION ALL

                          SELECT t.id, td.depth + 1
                          FROM tags t
                          JOIN tag_descendants td ON t.parent_tag_id = td.id
                      )
                      SELECT MIN(td.depth)
                      FROM tag_descendants td
                      JOIN content_tags ctg ON ctg.tags_id = td.id
                      JOIN book_content bc ON bc.content_id = ctg.contents_id
                      WHERE bc.book_id = b.id
                    ), 999)
                    """); //в прицнипе, задавая такое большое значение,
                //можно, наверное, дополнительно не проверять на точное наличие, но
                //это можно будеть оптимизировать при наличии проблем с производительностью   
            }
            var depthAvg = $"(({string.Join(" + ", depthSubqueries)}) / {requiredTagIds.Count}.0)";
            var scoringExpr = $"(1.0 / (1.0 + {depthAvg}))";
            var taggedSql = $"""
                    SELECT b.id AS id, {scoringExpr} AS best_similarity
                    FROM books b
                    WHERE {availFilter} 1=1
                    {filters}
                    """;
            return (taggedSql, parameters);
        }

        private static string BuildCommonFilters(ComplexSearchRequest request, List<object> parameters)
        {
            var sb = new StringBuilder();
            foreach (var pf in request.PersonFilters)
            {
                var pRole = parameters.Count;
                var pPersons = parameters.Count + 1;

                parameters.Add(pf.RoleId);
                parameters.Add(pf.PersonIds.ToArray());

                sb.AppendLine($$"""
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
                //bookFilters.AppendLine($$""" 
                //    AND EXISTS (
                //        SELECT 1
                //        FROM book_content bc
                //        JOIN content_theme ct ON ct.content_id = bc.content_id
                //        WHERE bc.book_id = b.id
                //          AND ct.theme_id = {{{p}}}
                //    ) 
                //    """);

                sb.AppendLine($$"""
                    AND EXISTS (
                       WITH RECURSIVE expanded_themes AS (
                          SELECT id FROM themes WHERE id = {{{p}}}
                          UNION
                          SELECT themes.id FROM themes
                          INNER JOIN expanded_themes ethe ON themes.parent_theme_id = ethe.id
                        )
                    SELECT 1
                    FROM book_content bc
                    JOIN content_theme cthe ON cthe.content_id = bc.content_id
                    WHERE bc.book_id = b.id
                       AND cthe.theme_id IN (SELECT id FROM expanded_themes)
                    )
                    """);
            }

            if (request.SelectionId > 0)
            {
                var p = parameters.Count;
                parameters.Add(request.SelectionId);
                sb.AppendLine($$""" 
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
                foreach (var tagId in request.RequiredTagIds)
                {
                    var p = parameters.Count;
                    parameters.Add(tagId);

                    sb.AppendLine($$"""
                        AND EXISTS (
                           WITH RECURSIVE expanded_tags AS (
                              SELECT id FROM tags WHERE id = {{{p}}}
                              UNION
                              SELECT t.id FROM tags t
                              INNER JOIN expanded_tags et ON t.parent_tag_id = et.id
                           )
                           SELECT 1
                           FROM book_content bc
                           JOIN content_tags ctg ON ctg.contents_id = bc.content_id
                           WHERE bc.book_id = b.id
                              AND ctg.tags_id IN (SELECT id FROM expanded_tags)
                        )
                        """);
                }
                //var p = parameters.Count;
                //parameters.Add(request.RequiredTagIds.ToArray());
                //bookFilters.AppendLine($$""" 
                //    AND EXISTS (
                //        SELECT 1
                //        FROM book_content bc
                //        JOIN content_tags ctg ON ctg.contents_id = bc.content_id
                //        WHERE bc.book_id = b.id
                //          AND ctg.tags_id = ANY({{{p}}})
                //    ) 
                //    """);
            }

            if (request.ExcludedTagIds.Count > 0)
            {
                var p = parameters.Count;
                parameters.Add(request.ExcludedTagIds.ToArray());

                sb.AppendLine($$"""
                    AND NOT EXISTS (
                       WITH RECURSIVE expanded_excluded_tags AS (
                          SELECT id FROM tags WHERE id = ANY({{{p}}})
                          UNION
                          SELECT t.id FROM tags t
                          INNER JOIN expanded_excluded_tags eet ON t.parent_tag_id = eet.id
                       )
                       SELECT 1
                       FROM book_content bc
                       JOIN content_tags ctg ON ctg.contents_id = bc.content_id
                       WHERE bc.book_id = b.id
                          AND ctg.tags_id IN (SELECT id FROM expanded_excluded_tags)
                    )
                    """);

                //bookFilters.AppendLine($$""" 
                //    AND NOT EXISTS (
                //        SELECT 1
                //        FROM book_content bc
                //        JOIN content_tags ctg ON ctg.contents_id = bc.content_id
                //        WHERE bc.book_id = b.id
                //          AND ctg.tags_id = ANY({{{p}}})
                //    ) 
                //    """);
            }

            return sb.ToString();

        }

        private async Task<Dictionary<long, BookSearchResult>> ProjectByIdsAsync(
            List<long> ids,
            long? userId,
            CancellationToken cancellationToken)
        {
            var favoriteShelfType = 1;
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
                    Authors = b.BookContents
                                .Select(bc=>bc.Content)
                                .SelectMany(c => c.Participations)
                                .Where(cp=> cp.PersonRoleId==1)
                                .GroupBy(cp => cp.PersonId)
                                .Select(group => group.First().Person.Name).ToList(),
                    //Where(...).Distinct(...).ToList() - на всякий случай оставлю второй вариант
                })
                .ToDictionaryAsync(b => b.Id, cancellationToken);
        }

        private sealed class SearchIdScore
        {
            public long Id { get; set; }
            public double BestSimilarity { get; set; }
        }


        private async Task<PagedBooks<BookSearchResult>> BuildPageAsync(
            List<SearchIdScore> pagedIds,
            int pageSize,
            long? userId,
            CancellationToken token)
        {
            var hasNext = pagedIds.Count > pageSize;
            var pageItems = pagedIds.Take(pageSize).ToList();
            if (pageItems.Count == 0)
                return new PagedBooks<BookSearchResult>
                {
                    Items = [],
                    HasNext = false,
                    LastId = null,
                    LastBestSimilarity = null,
                };

            var ids = pageItems.Select(x => x.Id).ToList();
            var itemsDict = await ProjectByIdsAsync(ids, userId, token);
            var items = ids.Where(idx => itemsDict.ContainsKey(idx))
                .Select(idx => itemsDict[idx]).ToList(); //по ids, чтобы сохранить порядок и проверить,
            //была ли она возвращена вторым запросом, эта книга
            var last = pageItems.Last();
            return new PagedBooks<BookSearchResult>
            {
                Items = items,
                HasNext = hasNext,
                LastId = last.Id,
                LastBestSimilarity = last.BestSimilarity
            };

        }

        public Task<List<PersonSuggestionDto>> GetPersonsByIdsAsync(
    List<long> ids, CancellationToken ct = default)
        {
            return _context.Persons
                .AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .Select(p => new PersonSuggestionDto { Id = (int)p.Id, Name = p.Name })
                .ToListAsync(ct);
        }
        //чтобы по илентификаторам получить названия тегов (для перехода по урлу, например)
        public async Task<List<TagSuggestionDto>> GetTagsByIdsAsync(
            List<long> ids, CancellationToken ct = default)
        {
            var tags = await _context.Tags
                .AsNoTracking()
                .Include(t => t.ParentTag)
                .Where(t => ids.Contains(t.Id))
                .ToListAsync(ct);

            var results = new List<TagSuggestionDto>(tags.Count);
            var seenIds = new HashSet<long>();

            foreach (var tag in tags)
            {
                bool isSynonym = tag.RelationTypeId == SynonymRelationTypeId
                                 && tag.ParentTagId.HasValue
                                 && tag.ParentTag is not null;

                var rootTag = isSynonym ? tag.ParentTag! : tag;
                var matchedName = isSynonym ? tag.Name : (string?)null;

                if (!seenIds.Add(rootTag.Id)) continue;

                results.Add(new TagSuggestionDto
                {
                    Id = (int)rootTag.Id,
                    Name = rootTag.Name,
                    MatchedName = matchedName,
                });
            }

            return results;
        }


        public Task<List<LanguageDto>> GetAllLanguagesAsync(CancellationToken ct = default)
        {
            return _context.Languages
                .AsNoTracking()
                .OrderBy(l => l.Name)
                .Select(l => new LanguageDto { Id = l.Id, Name = l.Name })
                .ToListAsync(ct);
        }

        public Task<List<CountryDto>> GetAllCountriesAsync(CancellationToken ct = default)
        {
            return _context.Countries
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CountryDto { Id = c.Id, Name = c.Name })
                .ToListAsync(ct);
        }

        public async Task<List<PersonRoleDto>> GetAllPersonRolesAsync(CancellationToken ct = default)
        {
            var res = await _context.PersonRoles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => new { Id = r.Id, Name = r.Name, Kind = r.Kind })
                .ToListAsync(ct);
            return res.Select(pr => new PersonRoleDto { Kind = (int)pr.Kind, Id = pr.Id, Name = pr.Name }).ToList();
        }

        public Task<List<PersonSuggestionDto>> SearchPersonsAsync(
            string name, int limit = 10, CancellationToken ct = default)
        {
            var pattern = $"%{name.Trim()}%";

            return _context.Persons
                .AsNoTracking()
                .Where(p => EF.Functions.ILike(p.Name, pattern))
                .OrderBy(p => p.Name)
                .Take(limit)
                .Select(p => new PersonSuggestionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    //ImagePath = p.ImagePath,
                })
                .ToListAsync(ct);
        }

        public async Task<List<TagSuggestionDto>> SearchTagsAsync(
            string name, int limit = 10, CancellationToken ct = default)
        {
            var pattern = $"%{name.Trim()}%";


            var found = await _context.Tags
                .AsNoTracking()
                .Include(t => t.ParentTag)
                .Include(t => t.TagType)
                .Where(t => EF.Functions.ILike(t.Name, pattern))
                .Take(limit * 2)
                .ToListAsync(ct);

            var results = new List<TagSuggestionDto>();
            var seenIds = new HashSet<long>();

            foreach (var tag in found)
            {
                bool isSynonym = tag.RelationTypeId == SynonymRelationTypeId
                                 && tag.ParentTagId.HasValue
                                 && tag.ParentTag is not null;

                var rootTag = isSynonym ? tag.ParentTag! : tag;
                var matchedName = isSynonym ? tag.Name : (string?)null;
                if (!seenIds.Add(rootTag.Id))
                    continue;

                results.Add(new TagSuggestionDto
                {
                    Id = rootTag.Id,
                    Name = rootTag.Name,
                    MatchedName = matchedName,
                    TagTypeName = tag.TagType.Name,
                });

                if (results.Count >= limit)
                    break;
            }

            return results;
        }
    }
}
