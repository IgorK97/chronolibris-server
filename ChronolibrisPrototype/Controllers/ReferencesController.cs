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

            var command = new CreateLanguageCommand(request.Name, request.FtsConfiguration);
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

            var command = new UpdateLanguageCommand(request.Id, request.Name, request.FtsConfiguration);
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
    }
}
