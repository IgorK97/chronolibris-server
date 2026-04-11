using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Application.Requests.References;
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
    public class PublishersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublishersController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherDto>>> GetAllPublishers(CancellationToken cancellationToken)
        {
            var query = new GetAllPublishersQuery();
            var publishers = await _mediator.Send(query, cancellationToken);
            return Ok(publishers);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherDto>> GetPublisherById(long id, CancellationToken cancellationToken)
        {
            var query = new GetPublisherByIdQuery(id);
            var publisher = await _mediator.Send(query, cancellationToken);

            if (publisher == null)
                return NotFound(new { message = $"Издательство с ID {id} не найдено" });

            return Ok(publisher);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<long>> CreatePublisher([FromBody] CreatePublisherRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Некорректные данные запроса", errors = ModelState });

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Название издательства обязательно" });

            if (string.IsNullOrWhiteSpace(request.Description))
                return BadRequest(new { message = "Описание издательства обязательно" });

            if (request.CountryId <= 0)
                return BadRequest(new { message = "ID страны должен быть указан" });

            var command = new CreatePublisherCommand(request.Name, request.Description, request.CountryId);
            var id = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetPublisherById), new { id = id }, id);
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePublisher(long id, [FromBody] UpdatePublisherRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Некорректные данные запроса", errors = ModelState });

            if (id != request.Id)
                return BadRequest(new { message = "ID в пути и теле запроса не совпадают" });

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Название издательства обязательно" });

            if (string.IsNullOrWhiteSpace(request.Description))
                return BadRequest(new { message = "Описание издательства обязательно" });

            if (request.CountryId <= 0)
                return BadRequest(new { message = "ID страны должен быть указан" });

            var command = new UpdatePublisherCommand(request.Id, request.Name, request.Description, request.CountryId);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound(new { message = $"Издательство с ID {id} не найдено" });

            return NoContent();
        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePublisher(long id, CancellationToken cancellationToken)
        {
            var command = new DeletePublisherCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound(new { message = $"Издательство с ID {id} не найдено" });

            return NoContent();
        }
    }
}