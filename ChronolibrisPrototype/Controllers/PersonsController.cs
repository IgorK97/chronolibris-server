using Chronolibris.Application.Handlers;
using Chronolibris.Application.Handlers.References;
using Chronolibris.Application.Models;
using ChronolibrisWeb.InputModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ChronolibrisWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PersonsController(IMediator mediator) => _mediator = mediator;


        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllPersonsQuery();
            var persons = await _mediator.Send(query, cancellationToken);
            return Ok(persons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id, CancellationToken token)
        {
            var person = await _mediator.Send(new GetPersonByIdQuery(id));
            return person != null ? Ok(person) : NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePersonInputModel request)
        {

            var command = new CreatePersonCommand(
                request.Name,
                request.Description
            );

            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }



        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdatePersonInputModel request)
        {


            var command = new UpdatePersonCommand(
                id,
                request.Name,
                request.Description
            );

            await _mediator.Send(command);
            return NoContent();

        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var command = new DeletePersonCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}