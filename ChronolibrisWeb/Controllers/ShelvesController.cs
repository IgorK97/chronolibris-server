using System.Security.Claims;
using Chronolibris.Application.Requests.Shelves;
using Chronolibris.Application.Requests.Users;
using ChronolibrisWeb.InputModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShelvesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShelvesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public record CreateShelfRequest(string Name);

        [HttpPost]
        [Authorize(Roles="reader")]
        public async Task<IActionResult> CreateShelf(CreateShelfRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _mediator.Send(new CreateShelfCommand(userId, request.Name));
            return Ok(result);
        }
        [HttpDelete("{shelfId}")]
        [Authorize(Roles ="reader")]
        public async Task<IActionResult> DeleteShelf(long shelfId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            await _mediator.Send(new DeleteShelfCommand(userId, shelfId));
            return Ok();
        }

        [HttpPut("{shelfId}")]
        [Authorize(Roles ="reader")]
        public async Task<IActionResult> UpdateShelf(long shelfId, UpdateShelfInputModel request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            await _mediator.Send(new UpdateShelfCommand(userId, shelfId, request.Name));
            return Ok();
        }

        [HttpGet("books/{bookId}")]
        public async Task<IActionResult> SeekBookInShelves(long bookId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _mediator.Send(new SeekBookInShelvesQuery(userId, bookId));

            return Ok(result);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserShelves()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _mediator.Send(new GetUserShelvesQuery(userId));
            return Ok(result);
        }
        [HttpGet("{shelfId}/books")]
        public async Task<IActionResult> GetShelfBooks(long userId, long shelfId, 
            long? lastId = null, int limit = 20)
        {
            if (limit < 1) limit = 20;
            else if (limit > 100) limit = 100;

            var result = await _mediator.Send(
                new GetShelfBooksQuery(shelfId, lastId, limit, userId));

            return Ok(result);
        }

        [HttpPost("{shelfId}/books/{bookId}")]
        [Authorize(Roles ="reader")]
        public async Task<IActionResult> AddBook(long shelfId, long bookId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            await _mediator.Send(
                new AddBookToShelfCommand(shelfId, bookId, userId));
            return Ok();
        }

        [HttpDelete("{shelfId}/books/{bookId}")]
        [Authorize(Roles = "reader")]

        public async Task<IActionResult> RemoveBook(long shelfId, long bookId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            await _mediator.Send(new RemoveBookFromShelfCommand(shelfId, bookId, userId));
            return Ok();
        }

    }
}
