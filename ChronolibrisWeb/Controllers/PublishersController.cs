using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.References;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisWeb.Controllers
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
        public async Task<ActionResult<PublisherDto?>> GetPublisherById(long id, CancellationToken cancellationToken)
        {
            var query = new GetPublisherByIdQuery(id);
            var publisher = await _mediator.Send(query, cancellationToken);

            return Ok(publisher);
        }

        [Authorize(Roles ="admin")]
        [HttpPost]
        public async Task<ActionResult<long>> CreatePublisher([FromBody] CreatePublisherRequest request, CancellationToken cancellationToken)
        {
            //Можно ли это указать только в самой сущности и как это называется?
            if (request.CountryId <= 0)
                return BadRequest(new { message = "ID страны должен быть указан" });

            var command = new CreatePublisherCommand(request.Name, request.Description, request.CountryId);
            var id = await _mediator.Send(command, cancellationToken);

            return Ok(id);
        }


        [Authorize(Roles ="admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePublisher(long id, [FromBody] UpdatePublisherRequest request, CancellationToken cancellationToken)
        {

            if (request.CountryId <= 0)
                return BadRequest(new { message = "ID страны должен быть указан" });

            var command = new UpdatePublisherCommand(request.Id, request.Name, request.Description, request.CountryId);
            var result = await _mediator.Send(command, cancellationToken);

            return NoContent();
        }


        [Authorize(Roles ="admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(long id, CancellationToken cancellationToken)
        {
            var command = new DeletePublisherCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}