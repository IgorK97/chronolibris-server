// File: ChronolibrisPrototype.Controllers.SeriesController.cs
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChronolibrisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SeriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получает список всех серий книг
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SeriesDto>>> GetAllSeries(CancellationToken cancellationToken)
        {
            var query = new GetAllSeriesQuery();
            var series = await _mediator.Send(query, cancellationToken);
            return Ok(series);
        }

        /// <summary>
        /// Получает серию по идентификатору
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SeriesDto>> GetSeriesById(long id, CancellationToken cancellationToken)
        {
            var query = new GetSeriesByIdQuery(id);
            var series = await _mediator.Send(query, cancellationToken);

            if (series == null)
                return NotFound(new { message = $"Серия с ID {id} не найдена" });

            return Ok(series);
        }

        /// <summary>
        /// Создает новую запись серии книг
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<long>> CreateSeries([FromBody] CreateSeriesRequest request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Название серии обязательно" });

            if (request.PublisherId <= 0)
                return BadRequest(new { message = "ID издательства должен быть указан" });

            var command = new CreateSeriesCommand(request.Name, request.PublisherId);
            var id = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetSeriesById), new { id = id }, id);
        }

        /// <summary>
        /// Обновляет существующую запись серии книг
        /// </summary>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSeries(long id, [FromBody] UpdateSeriesRequest request, CancellationToken cancellationToken)
        {

            if (id != request.Id)
                return BadRequest(new { message = "ID в пути и теле запроса не совпадают" });

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Название серии обязательно" });

            if (request.PublisherId <= 0)
                return BadRequest(new { message = "ID издательства должен быть указан" });

            var command = new UpdateSeriesCommand(request.Id, request.Name, request.PublisherId);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound(new { message = $"Серия с ID {id} не найдена" });

            return NoContent();
        }

        /// <summary>
        /// Удаляет запись серии книг
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSeries(long id, CancellationToken cancellationToken)
        {
            var command = new DeleteSeriesCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound(new { message = $"Серия с ID {id} не найдена" });

            return NoContent();
        }
    }
}