
using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Models.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Chronolibris.API.Controllers.Search
{

    [ApiController]
    [Route("api/search/reference")]
    public class SearchReferenceController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SearchReferenceController(IMediator mediator) => _mediator = mediator;


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
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Параметр name обязателен.");

            if (limit is < 1 or > 50)
                return BadRequest("limit должен быть от 1 до 50.");

            return await _mediator.Send(new SearchPersonsQuery(name, limit), ct);
        }


        [HttpGet("tags")]
        public async Task<ActionResult<List<TagSuggestionDto>>> SearchTags(
            [FromQuery] string name,
            [FromQuery] int limit = 10,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Параметр name обязателен.");

            if (limit is < 1 or > 50)
                return BadRequest("limit должен быть от 1 до 50.");

            return await _mediator.Send(new SearchTagsQuery(name, limit), ct);
        }

        [HttpGet("persons-batch")]
        public async Task<ActionResult<List<PersonSuggestionDto>>> GetPersonsByIds(
            [FromQuery] string ids,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return BadRequest("Параметр ids обязателен.");

            var parsedIds = ParseLongIds(ids);
            if (parsedIds is null)
                return BadRequest("ids должны быть целыми числами, разделёнными запятой.");

            if (parsedIds.Count > 50)
                return BadRequest("Максимум 50 id за запрос.");

            return await _mediator.Send(new GetPersonsByIdsQuery(parsedIds), ct);
        }

        [HttpGet("tags-batch")]
        public async Task<ActionResult<List<TagSuggestionDto>>> GetTagsByIds(
            [FromQuery] string ids,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return BadRequest("Параметр ids обязателен.");

            var parsedIds = ParseLongIds(ids);
            if (parsedIds is null)
                return BadRequest("ids должны быть целыми числами, разделёнными запятой.");

            if (parsedIds.Count > 50)
                return BadRequest("Максимум 50 id за запрос.");

            return await _mediator.Send(new GetTagsByIdsQuery(parsedIds), ct);
        }

        private static List<long>? ParseLongIds(string raw)
        {
            var result = new List<long>();
            foreach (var part in raw.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!long.TryParse(part.Trim(), out var id))
                    return null;
                result.Add(id);
            }
            return result.Count > 0 ? result : null;
        }
    }
}
