
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Application.Requests.References;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChronolibrisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ThemesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ThemeDto>>> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllThemesQuery(null);
            var themes = await _mediator.Send(query, cancellationToken);
            return Ok(themes);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThemeDto>>> GetAllByName([FromQuery] string q, CancellationToken cancellationToken)
        {
            var themes = await _mediator.Send(new GetThemesByNameQuery(q), cancellationToken);
            return Ok(themes);
        }

        [HttpGet("parent/{parentThemeId}")]
        public async Task<ActionResult<IEnumerable<ThemeDto>>> GetByParentId(long parentThemeId, CancellationToken cancellationToken)
        {
            var query = new GetAllThemesQuery(parentThemeId);
            var themes = await _mediator.Send(query, cancellationToken);
            return Ok(themes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ThemeDto>> GetById(long id, CancellationToken cancellationToken)
        {
            var query = new GetThemeByIdQuery(id);
            var theme = await _mediator.Send(query, cancellationToken);

            if (theme == null)
                return NotFound(new { message = $"Тема с ID {id} не найдена" });

            return Ok(theme);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] CreateThemeRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Некорректные данные запроса", errors = ModelState });

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Название темы обязательно" });

            try
            {
                var command = new CreateThemeCommand(request.Name, request.ParentThemeId);
                var id = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = id }, id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(long id, [FromBody] UpdateThemeRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Некорректные данные запроса", errors = ModelState });

            if (id != request.Id)
                return BadRequest(new { message = "ID в пути и теле запроса не совпадают" });

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Название темы обязательно" });

            try
            {
                var command = new UpdateThemeCommand(id, request.Name, request.ParentThemeId);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Тема с ID {id} не найдена" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteThemeCommand(id);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Тема с ID {id} не найдена" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}