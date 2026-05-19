using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Books;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChronolibrisWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookFilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookFilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<List<BookFileDto>>> GetBookFiles(long bookId, [FromQuery] bool mode, CancellationToken cancellationToken)
        {
            bool adminMode = mode;
            var query = new GetBookFilesQuery(bookId, adminMode);
            var files = await _mediator.Send(query, cancellationToken);
            return Ok(files);
        }

        [HttpGet("{id}/download")]
        public async Task<ActionResult> DownloadBookFile(long id, CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId) || !TryGetRole(out var role))
                return Unauthorized();
            var query = new GetBookFileQuery(id, userId, role);
            var stream = await _mediator.Send(query, cancellationToken);

            if (stream == null)
                return NotFound();

            return File(stream, "application/octet-stream", $"book_file_{id}");
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [RequestSizeLimit(50 * 1024 * 1024)]
        public async Task<ActionResult<long>> UploadBookFile(
            [FromForm] long bookId,
            [FromForm] int formatId,
            [FromForm] bool isReadable,
            [FromForm] bool? historical,
            IFormFile file,
            CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Файл не предоставлен" });

            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var command = new UploadBookFileCommand(bookId, formatId, isReadable, historical, file.OpenReadStream(), file.FileName, file.Length, userId);
            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);

        }

        [Authorize(Roles = "admin, moderator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBookFile(long id, CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { message = "Пользователь не авторизован" });

            var command = new DeleteBookFileCommand(id, userId);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        private bool TryGetUserId(out long userId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return long.TryParse(claim?.Value, out userId);
        }

        private bool TryGetRole(out string role)
        {
            var claim = User.FindFirst(ClaimTypes.Role);
            if (claim == null)
            {
                role = "";
                return false;
            }
            role = claim.Value;
            return true;
        }
    }
}