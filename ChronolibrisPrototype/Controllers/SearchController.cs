using System.Security.Claims;
using Chronolibris.Application.Requests.Search;
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

        [HttpGet]
        public async Task<ActionResult<PagedResult<BookSearchResult>>> Search(
            [FromQuery] SimpleSearchHttpRequest request, bool mode = false,
            CancellationToken cancellationToken = default)
        {
            if (request.LastBestSimilarity.HasValue != request.LastId.HasValue)
                return BadRequest(
                    "LastBestSimilarity и LastId должны передаваться вместе.");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                //return Unauthorized();
                userId = 0;
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            if (mode && (userId == 0 || roleClaim != "admin"))
                return BadRequest();

            var result = await _mediator.Send(
                new SimpleSearchKeysetQuery(
                    Query: request.Query,
                    PageSize: request.PageSize,
                    UserId: userId,
                    LastBestSimilarity: request.LastBestSimilarity,
                    LastId: request.LastId,
                    mode),
                cancellationToken);

            return Ok(result);
        }


        [HttpPost("advanced")]
        public async Task<ActionResult<PagedResult<BookSearchResult>>> AdvancedSearch(
            [FromBody] AdvancedSearchInputModel request, bool hiddenIsAvailableMode=false,
            CancellationToken cancellationToken = default)
        {
            if (request.LastBestSimilarity.HasValue != request.LastId.HasValue)
                return BadRequest(
                    "LastBestSimilarity и LastId должны передаваться вместе.");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                //return Unauthorized();
                userId = 0;
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            if (hiddenIsAvailableMode && (userId == 0 || roleClaim != "admin"))
                return BadRequest();

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
                    SelectionId:request.SelectionId,
                    mode:hiddenIsAvailableMode),
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