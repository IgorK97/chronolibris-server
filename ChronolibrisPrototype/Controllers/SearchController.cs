using System.Security.Claims;
using Chronolibris.Application.Search.Queries;
using Chronolibris.Domain.Models.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Chronolibris.API.Controllers.Search
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Простой поиск по названию книги или её контентов.
        /// Первая страница: GET /api/search?query=война&amp;pageSize=20
        /// Следующая страница: GET /api/search?query=война&amp;pageSize=20
        ///     &amp;lastBestSimilarity=0.75&amp;lastId=42
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<BookSearchResult>>> Search(
            [FromQuery] SimpleSearchHttpRequest request,
            CancellationToken cancellationToken)
        {
            // Курсор валиден только если переданы оба поля одновременно
            if (request.LastBestSimilarity.HasValue != request.LastId.HasValue)
                return BadRequest(
                    "LastBestSimilarity и LastId должны передаваться вместе.");

            var result = await _mediator.Send(
                new SimpleSearchKeysetQuery(
                    Query: request.Query,
                    PageSize: request.PageSize,
                    UserId: TryGetUserId(),
                    LastBestSimilarity: request.LastBestSimilarity,
                    LastId: request.LastId),
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Расширенный поиск с фильтрами.
        /// Первая страница: POST /api/search/advanced, тело без курсора.
        /// Следующая страница: то же тело + lastBestSimilarity и lastId из предыдущего ответа.
        /// </summary>
        [HttpPost("advanced")]
        public async Task<ActionResult<PagedResult<BookSearchResult>>> AdvancedSearch(
            [FromBody] AdvancedSearchInputModel request,
            CancellationToken cancellationToken)
        {

            var personFilters = request.PersonFilters
                .Select(f => new PersonRoleFilter
                {
                    RoleId = f.RoleId,
                    PersonIds = f.PersonIds,
                })
                .ToList();

            var result = await _mediator.Send(
                new AdvancedSearchKeysetQuery(
                    Query: request.Query,
                    PageSize: request.PageSize,
                    UserId: TryGetUserId(),
                    LastBestSimilarity: request.LastBestSimilarity,
                    LastId: request.LastId,
                    PersonFilters: personFilters,
                    RequiredTagIds: request.RequiredTagIds,
                    ExcludedTagIds: request.ExcludedTagIds,
                    ThemeId: request.ThemeId,
                    SelectionId:request.SelectionId),
                cancellationToken);

            return Ok(result);
        }

        private long? TryGetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(claim, out var id) ? id : null;
        }
    }
}