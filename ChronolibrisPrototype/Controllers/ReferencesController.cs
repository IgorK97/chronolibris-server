using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferencesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReferencesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получает полный справочник ролей участников (авторы, редакторы, переводчики и т.д.).
        /// </summary>
        /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
        /// <returns>Список объектов RoleDetails.</returns>
        [HttpGet("roles")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RoleDetails>))]
        public async Task<ActionResult<IEnumerable<RoleDetails>>> GetPersonRoles()
        {
            var query = new GetRoleDetailsQuery();

            var roles = await _mediator.Send(query);

            return Ok(roles);
        }

        /// <summary>
        /// Получает список всех доступных FTS конфигураций из PostgreSQL
        /// </summary>
        [HttpGet("fts-configurations")]
        public async Task<ActionResult<IEnumerable<FtsConfigurationDto>>> GetFtsConfigurations(CancellationToken cancellationToken)
        {
            var query = new GetFtsConfigurationsQuery();
            var configurations = await _mediator.Send(query, cancellationToken);
            return Ok(configurations);
        }


        /// <summary>
        /// Получает список всех языков
        /// </summary>
        [HttpGet("languages")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LanguageDto>))]
        public async Task<ActionResult<IEnumerable<LanguageDto>>> GetAllLanguages(CancellationToken cancellationToken)
        {
            var query = new GetAllLanguagesQuery();
            var languages = await _mediator.Send(query, cancellationToken);
            return Ok(languages);
        }

        /// <summary>
        /// Получает язык по идентификатору
        /// </summary>
        [HttpGet("languages/{id}")]
        public async Task<ActionResult<LanguageDto>> GetLanguageById(long id, CancellationToken cancellationToken)
        {
            var query = new GetLanguageByIdQuery(id);
            var language = await _mediator.Send(query, cancellationToken);

            return Ok(language);
        }

        /// <summary>
        /// Создает новую запись языка
        /// </summary>
        [Authorize]
        [HttpPost("languages")]
        public async Task<ActionResult<long>> CreateLanguage([FromBody] CreateLanguageRequest request, CancellationToken cancellationToken)
        {

            var command = new CreateLanguageCommand(request.Name);
            var id = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetLanguageById), new { id = id }, id);
        }
        /// <summary>
        /// Обновляет существующую запись языка
        /// </summary>
        [Authorize]
        [HttpPut("languages/{id}")]
        public async Task<ActionResult> UpdateLanguage(long id, [FromBody] UpdateLanguageRequest request, CancellationToken cancellationToken)
        {

            var command = new UpdateLanguageCommand(request.Id, request.Name);
            var result = await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Удаляет запись языка
        /// </summary>
        [Authorize]
        [HttpDelete("languages/{id}")]
        public async Task<ActionResult> DeleteLanguage(long id, CancellationToken cancellationToken)
        {
            var command = new DeleteLanguageCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Получает список всех стран
        /// </summary>
        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountries(CancellationToken cancellationToken)
        {
            var query = new GetAllCountriesQuery();
            var countries = await _mediator.Send(query, cancellationToken);
            return Ok(countries);
        }

        /// <summary>
        /// Получает страну по идентификатору
        /// </summary>
        [HttpGet("countries/{id}")]
        public async Task<ActionResult<CountryDto>> GetCountryById(long id, CancellationToken cancellationToken)
        {
            var query = new GetCountryByIdQuery(id);
            var country = await _mediator.Send(query, cancellationToken);

            if (country == null)
                return NotFound(new { message = $"Страна с ID {id} не найдена" });

            return Ok(country);
        }

        /// <summary>
        /// Создает новую запись страны
        /// </summary>
        [Authorize]
        [HttpPost("countries")]
        public async Task<ActionResult<long>> CreateCountry([FromBody] CreateCountryRequest request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Название страны обязательно" });

            var command = new CreateCountryCommand(request.Name);
            var id = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetCountryById), new { id = id }, id);
        }

        /// <summary>
        /// Обновляет существующую запись страны
        /// </summary>
        [Authorize]
        [HttpPut("countries/{id}")]
        public async Task<ActionResult> UpdateCountry(long id, [FromBody] UpdateCountryRequest request, CancellationToken cancellationToken)
        {

            if (id != request.Id)
                return BadRequest(new { message = "ID в пути и теле запроса не совпадают" });

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Название страны обязательно" });

            var command = new UpdateCountryCommand(request.Id, request.Name);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound(new { message = $"Страна с ID {id} не найдена" });

            return NoContent();
        }

        /// <summary>
        /// Удаляет запись страны
        /// </summary>
        [Authorize]
        [HttpDelete("countries/{id}")]
        public async Task<ActionResult> DeleteCountry(long id, CancellationToken cancellationToken)
        {
            var command = new DeleteCountryCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound(new { message = $"Страна с ID {id} не найдена" });

            return NoContent();
        }

        /// <summary>
        /// Получает список всех форматов книг
        /// </summary>
        [HttpGet("formats")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FormatDto>))]
        public async Task<ActionResult<IEnumerable<FormatDto>>> GetAllFormats(CancellationToken cancellationToken)
        {
            var query = new GetAllFormatsQuery();
            var formats = await _mediator.Send(query, cancellationToken);
            return Ok(formats);
        }

        /// <summary>
        /// Получает формат по идентификатору
        /// </summary>
        [HttpGet("formats/{id}")]
        public async Task<ActionResult<FormatDto>> GetFormatById(int id, CancellationToken cancellationToken)
        {
            var query = new GetFormatByIdQuery(id);
            var format = await _mediator.Send(query, cancellationToken);

            if (format == null)
                return NotFound(new { message = $"Формат с ID {id} не найден" });

            return Ok(format);
        }

        /// <summary>
        /// Создает новую запись формата
        /// </summary>
        [Authorize]
        [HttpPost("formats")]
        public async Task<ActionResult<int>> CreateFormat([FromBody] CreateFormatRequest request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Название формата обязательно" });

            var command = new CreateFormatCommand(request.Name);
            var id = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetFormatById), new { id = id }, id);
        }

        /// <summary>
        /// Обновляет существующую запись формата
        /// </summary>
        [Authorize]
        [HttpPut("formats/{id}")]
        public async Task<ActionResult> UpdateFormat(int id, [FromBody] UpdateFormatRequest request, CancellationToken cancellationToken)
        {

            if (id != request.Id)
                return BadRequest(new { message = "ID в пути и теле запроса не совпадают" });

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Название формата обязательно" });

            var command = new UpdateFormatCommand(request.Id, request.Name);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound(new { message = $"Формат с ID {id} не найден" });

            return NoContent();
        }

        /// <summary>
        /// Удаляет запись формата
        /// </summary>
        [Authorize]
        [HttpDelete("formats/{id}")]
        public async Task<ActionResult> DeleteFormat(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteFormatCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound(new { message = $"Формат с ID {id} не найден" });

            return NoContent();
        }


    }
}
