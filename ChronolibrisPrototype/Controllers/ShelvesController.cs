using System.Security.Claims;
using Chronolibris.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisPrototype.Controllers
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
        public async Task<IActionResult> CreateShelf(CreateShelfRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _mediator.Send(new CreateShelfCommand(userId, request.Name));
            return Ok(result);
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

        //public record AddBookToShelf(long shelfId, long bookId);

        [HttpPost("{shelfId}/books/{bookId}")]
        public async Task<IActionResult> AddBook(long shelfId, long bookId)
        {
            bool res = await _mediator.Send(new AddBookToShelfCommand(shelfId, bookId));
            return Ok(res);
        }

        //public record DeleteBookFromShelf(long shelfId, long bookId);

        [HttpDelete("{shelfId}/books/{bookId}")]
        public async Task<IActionResult> RemoveBook(long shelfId, long bookId)
        {
            bool res = await _mediator.Send(new RemoveBookFromShelfCommand(shelfId, bookId));
            return Ok(res);
        }

    }
}
