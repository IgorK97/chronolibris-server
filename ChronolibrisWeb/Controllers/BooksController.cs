using System.Security.Claims;
using Chronolibris.Application.Handlers.Books;
using Chronolibris.Application.Requests.Books;
using Chronolibris.Domain.Models;
using ChronolibrisWeb.InputModels;
using ChronolibrisWeb.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<long>> CreateBook(
            [FromBody] CreateBookInputModel request, CancellationToken cancellationToken)
        {
            if (!ControllerUtils.TryGetUserId(User, out var userId))
                return Unauthorized();
            var command = new CreateBookCommand(
                userId,
                request.Title,
                request.Description,
                request.CountryId,
                request.LanguageId,
                request.Year,
                request.ISBN,
                request.Bbk,
                request.Udk,
                request.Source,
                request.CoverBase64,
                request.IsAvailable,
                request.IsReviewable,
                request.PublisherId,
                request.HasHistoricalVersions,
                request.PersonFilters
            );

            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);

        }

        [Authorize(Roles = "admin, moderator")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBook(
            long id,
            [FromBody] UpdateBookInputModel request,
            CancellationToken cancellationToken)
        {
            if (!ControllerUtils.TryGetUserId(User, out var userId))
                return Unauthorized();
            var command = new UpdateBookCommand
            (id, userId, request.OldUpdatedAt,
               request.Title,
                request.Description,
                request.CountryId,
                request.LanguageId,
                request.Year, request.YearProvided,
                request.ISBN, request.IsbnProvided,
                request.Bbk, request.BbkProvided,
                request.Udk, request.UdkProvided,
                request.Source, request.SourceProvided,
                request.CoverBase64,
                request.IsAvailable,
                request.IsReviewable,
                request.PublisherId, request.PublisherIdProvided,
                request.PersonFilters,
                request.ThemeIds,
                request.DeleteCoverCommand,
                request.HasHistoricalVersions
            );

            await _mediator.Send(command, cancellationToken);
            return Ok();

        }

        [HttpGet("{bookId}/info")]
        public async Task<ActionResult> GetBookMetadata(long bookId, bool mode)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
            //return Unauthorized();
            {
                userId = 0;


            }
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            if (mode && (userId == 0 || !((roleClaim == "admin") || (roleClaim == "moderator"))))
            {
                return BadRequest();
            }
            var metadata = await _mediator.Send(new GetBookMetadataQuery(bookId, userId, mode));
            return Ok(metadata);
        }

        [HttpGet("{id}/contents")]
        public async Task<ActionResult<List<ContentDto>>> GetBookContents(long id, CancellationToken cancellationToken)
        {
            var query = new GetBookContentsQuery(id);
            var contents = await _mediator.Send(query, cancellationToken);
            return Ok(contents);
        }

        [HttpGet("files/{bookFileId}/toc")]
        public async Task<ActionResult> GetToc(long bookFileId, CancellationToken ct)
        {
            if (!TryGetUserId(out var userId) || !TryGetRole(out var role))
                return Unauthorized();
            var json = await _mediator.Send(new GetTocQuery(bookFileId, userId, role), ct);
            if (json is null)
                return NotFound();

            return Content(json, "application/json; charset=utf-8");

        }

        [HttpGet("files/{bookFileId}/chunks/{chunkIndex}")]
        public async Task<ActionResult> GetChunk(long bookFileId, string chunkIndex, CancellationToken ct)
        {
            if (!TryGetUserId(out var userId) || !TryGetRole(out var role))
                return Unauthorized();
            var json = await _mediator.Send(new GetChunkQuery(bookFileId, chunkIndex, userId, role), ct);
            if (json is null)
                return NotFound(new { message = $"Фрагмент {chunkIndex} не найден" });

            return Content(json, "application/json; charset=utf-8");

        }

        [HttpGet("images/{bookfileId}/{fileName}")]
        public async Task<ActionResult> GetImage(long bookFileId, string fileName, CancellationToken ct)
        {
            if (!TryGetUserId(out var userId) || !TryGetRole(out var role))
                return Unauthorized();
            var stream = await _mediator.Send(
                new GetBookImageQuery(bookFileId, fileName, userId, role), ct);

            if (stream is null)
                return NotFound();

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = ext switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            return File(stream, contentType);
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
