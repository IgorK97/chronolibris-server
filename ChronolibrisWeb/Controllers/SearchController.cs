using System.Security.Claims;
using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Models;
using Chronolibris.Domain.Models.Search;
using ChronolibrisWeb.InputModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisWeb.Controllers
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
        public async Task<ActionResult<PagedBooks<BookSearchResult>>> Search(
            [FromQuery] SimpleSearchInputModel request, bool mode = false,
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
        public async Task<ActionResult<PagedBooks<BookSearchResult>>> AdvancedSearch(
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
                    Mode:hiddenIsAvailableMode),
                cancellationToken);

            return Ok(result);
        }

        private long? TryGetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(claim, out var id) ? id : null;
        }

        [HttpGet("languages")]
        public Task<List<LanguageDto>> GetLanguages(CancellationToken ct)
           => _mediator.Send(new GetLanguagesQuery(), ct);

        [HttpGet("countries")]
        public Task<List<CountryDto>> GetCountries(CancellationToken ct)
            => _mediator.Send(new GetCountriesQuery(), ct);


        [HttpGet("person-roles")]
        public Task<List<PersonRoleDto>> GetPersonRoles(CancellationToken ct)
            => _mediator.Send(new GetPersonRolesQuery(), ct);



        [HttpGet("persons")]
        public async Task<ActionResult<List<PersonSuggestionDto>>> SearchPersons(
            [FromQuery] string name,
            [FromQuery] int limit = 10,
            CancellationToken ct = default)
        {

            if (limit is < 1 or > 50)
                limit = 10;

            return await _mediator.Send(new SearchPersonsQuery(name, limit), ct);
        }


        [HttpGet("tags")]
        public async Task<ActionResult<List<TagSuggestionDto>>> SearchTags(
            [FromQuery] string name,
            [FromQuery] int limit = 10,
            CancellationToken ct = default)
        {

            if (limit is < 1 or > 50)
                limit = 10;

            return await _mediator.Send(new SearchTagsQuery(name, limit), ct);
        }

        [HttpGet("persons-batch")]
        public async Task<ActionResult<List<PersonSuggestionDto>>> GetPersonsByIds(
            [FromQuery] PersonInputModel personInputModel,
            CancellationToken ct)
        {
            return await _mediator.Send(new GetPersonsByIdsQuery(personInputModel.ParsedIds), ct);
        }

        [HttpGet("tags-batch")]
        public async Task<ActionResult<List<TagSuggestionDto>>> GetTagsByIds(
            [FromQuery] TagsInputModel tagsInputModel,
            CancellationToken ct)
        {
            return await _mediator.Send(new GetTagsByIdsQuery(tagsInputModel.ParsedIds), ct);
        }
    }
}