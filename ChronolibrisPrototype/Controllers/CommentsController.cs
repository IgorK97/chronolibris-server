using System.Security.Claims;
using Chronolibris.Application.Handlers;
using Chronolibris.Application.Handlers.Comments;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Application.Requests.Comments;
using Chronolibris.Domain.Models;
using ChronolibrisPrototype.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CommentsController(IMediator mediator) => _mediator = mediator;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCommentRequest request)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var id = await _mediator.Send(new CreateCommentCommand(
                request.BookId, userId, request.Text, request.ParentCommentId));
            return Ok(id);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            await _mediator.Send(new DeleteCommentCommand(id, userId));
            return NoContent();
        }

        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<List<CommentDto>>> GetBookComments(
            long bookId, long? lastId, int limit = 20)
        {
            if (!TryGetUserId(out var userId)) 
                userId = 0;
            var result = await _mediator.Send(new GetBookCommentsQuery(bookId, lastId, limit, userId));
            return Ok(result);
        }

        [HttpGet("{parentId}/replies")]
        public async Task<ActionResult<List<CommentDto>>> GetReplies(
            long parentId, long? lastId, int limit = 20)
        {
            if (!TryGetUserId(out var userId)) userId = 0;
            var result = await _mediator.Send(new GetCommentRepliesQuery(parentId, lastId, limit, userId));
            return Ok(result);
        }

        private bool TryGetUserId(out long userId)
        {
            return long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        }

        [Authorize]
        [HttpPost("rate")]
        public async Task<IActionResult> RateComment(RateCommentRequest request)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var result = await _mediator.Send(
                new RateCommentCommand
                {
                    CommentId = request.CommentId,
                    Score = request.Score,
                    UserId = userId
                });

            return Ok(result);
        }
    }
}
