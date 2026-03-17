using System.Security.Claims;
using Chronolibris.Application.Requests;
using ChronolibrisPrototype.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisPrototype.Controllers
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
        public async Task<IActionResult> Add([FromBody] AddBookmarkRequest command)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _mediator.Send(new AddBookmarkCommand(command.bookFileId,userId,command.noteText, command.paraIndex));
            if (result<=0) return BadRequest();
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Remove(RemoveBookmarkCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result) return NotFound(0);
            return Ok();
        }

        /// <summary>
        /// Обновляет заметку к закладке
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> UpdateBookmark(
            long id,
            [FromBody] UpdateBookmarkRequest request,
            CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var command = new UpdateBookmarkCommand(id, userId, request.Note);
            var result = await _mediator.Send(command, cancellationToken);

            return result ? Ok() : NotFound();
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
