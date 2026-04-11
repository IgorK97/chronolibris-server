using Chronolibris.Application.Handlers;
using Chronolibris.Application.Handlers.References;
using Chronolibris.Application.Models;
using ChronolibrisPrototype.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonRequest request)
    {

        var command = new CreatePersonCommand(
            request.Name,
            request.Description
        );

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken token)
    {
        var person = await _mediator.Send(new GetPersonByIdQuery(id));
        return person != null ? Ok(person) : NotFound();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePersonRequest request)
    {


        var command = new UpdatePersonCommand(
            id,
            request.Name,
            request.Description
        );

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}