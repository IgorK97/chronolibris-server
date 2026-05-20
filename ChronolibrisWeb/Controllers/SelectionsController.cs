using System.Diagnostics.Contracts;
using System.Security.Claims;
using Chronolibris.Application.Requests.Selections;
using ChronolibrisWeb.InputModels;
using ChronolibrisWeb.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelectionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SelectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetSelections(long? lastId = null,
            int limit = 20,
            bool? onlyActive = true)
        {
            if (onlyActive != true)
            {
                var role = User.FindFirstValue(ClaimTypes.Role);
                if (role != "admin")
                    return Forbid();
            }

            if (limit < 1) limit = 20;
            else if (limit > 100) limit = 100;

            var result = await _mediator.Send(new GetSelectionsQuery(lastId, limit, onlyActive));
            return Ok(result);
        }

        [HttpGet("{selectionId}")]
        public async Task<IActionResult> GetSelection(long selectionId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userRole;
            if (!long.TryParse(userIdClaim, out var userId))
            {
                userId = 0;
                userRole = "";
            }
            else
                userRole = User.FindFirstValue(ClaimTypes.Role) ?? "";


            var selection = await _mediator.Send(new GetSelectionQuery(selectionId, userId, userRole));
            return Ok(selection);
        }

        [HttpPost]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> CreateSelection([FromBody] CreateSelectionInputModel request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
            {
                return Forbid();
            }
            var selectionId = await _mediator.Send(new CreateSelectionRequest(request.Name, request.Description, userId));
            return Ok(selectionId);
        }

        [HttpPut("{selectionId}")]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> UpdateSelection(long selectionId, [FromBody] UpdateSelectionInputModel request)
        {
            if (!ControllerUtils.TryGetUserId(User, out var userId))
                return Unauthorized();
            var result = await _mediator.Send(new UpdateSelectionRequest(request.SelectionId, request.Name, request.Description, request.IsActive,userId));
            return NoContent();
        }

        [HttpDelete("{selectionId}")]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> DeleteSelection(long selectionId)
        {
            var result = await _mediator.Send(new DeleteSelectionRequest(selectionId));
            return NoContent();
        }

        [HttpPost("{selectionId}/books/{bookId}")]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> AddBook(long selectionId, long bookId, CancellationToken ct)
        {
            await _mediator.Send(new AddBookToSelectionRequest(selectionId, bookId), ct);
            return Ok();
        }

        [HttpDelete("{selectionId}/books/{bookId}")]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> RemoveBook(long selectionId, long bookId)
        {
            var result = await _mediator.Send(new RemoveBookFromSelectionRequest(selectionId, bookId));
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetSelections()
        {
            var result = await _mediator.Send(new GetAllSelectionsQuery());
            return Ok(result);
        }

        [HttpGet("{selectionId}/books")]
        //[Authorize]
        public async Task<IActionResult> GetBooks(long selectionId, long? lastId, int limit = 20, bool mode = false)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                //return Unauthorized();
                userId = 0;
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            if (mode && (userId == 0 || roleClaim != "admin"))
                return BadRequest();
            if (limit < 1) limit = 20;
            else if (limit > 100) limit = 100;

            var result = await _mediator.Send(
                new GetSelectionBooksQuery(selectionId, lastId, limit, userId, mode));

            return Ok(result);
        }

        [HttpGet("books/{bookId}")]
        public async Task<IActionResult> SeekBookInSelection(long bookId, CancellationToken ct)
        {
            var result = await _mediator.Send(new SeekBookInSelectionsQuery(bookId), ct);
            return Ok(result);
        }

    }
}
