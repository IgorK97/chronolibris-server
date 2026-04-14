using System.Security.Claims;
using Chronolibris.Application.Requests.Bookmarks;
using ChronolibrisWeb.InputModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookmarksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddBookmarkInputModel command)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _mediator.Send(new AddBookmarkCommand(command.bookFileId,userId,command.noteText, command.paraIndex));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(long id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            await _mediator.Send(new RemoveBookmarkCommand(id, userId));
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBookmark(
            long id,
            [FromBody] UpdateBookmarkInputModel request,
            CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var command = new UpdateBookmarkCommand(id, userId, request.Note);
            await _mediator.Send(command, cancellationToken);

            return Ok();
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetAll(long bookId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var bookmarks = await _mediator.Send(new GetBookmarksQuery(bookId, userId));
            return Ok(bookmarks);
        }
    }
}
