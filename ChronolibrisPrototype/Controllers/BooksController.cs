using System.Security.Claims;
using Chronolibris.Application.Handlers.Books;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Books;
using Chronolibris.Domain.Models;
using ChronolibrisWeb.InputModels;
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

        [HttpGet]
        public async Task<ActionResult<BookListResponse>> GetBooks(
            [FromQuery] BookFilterRequest filter, CancellationToken cancellationToken)
        {
            var query = new GetBooksQuery(filter);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBookById(long id, CancellationToken cancellationToken)
        {
            var query = new GetBookByIdQuery(id);
            var book = await _mediator.Send(query, cancellationToken);

            if (book == null)
                return NotFound(new { message = $"Книга с ID {id} не найдена" });

            return Ok(book);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<long>> CreateBook(
            [FromBody] CreateBookRequest request, CancellationToken cancellationToken)
        {

            var command = new CreateBookCommand(
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
                request.CoverContentType,
                request.CoverFileName,
                request.IsAvailable,
                request.IsReviewable,
                request.PublisherId,
                request.SeriesId,
                request.PersonFilters,
                request.ThemeIds
            );

            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);

        }

        [Authorize(Roles = "admin, moderator")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBook(
            long id,
            [FromBody] Chronolibris.Application.Handlers.Books.UpdateBookRequest request,
            CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest(new { message = "ID в пути и теле запроса не совпадают" });

            var command = new UpdateBookCommand
            (id,
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
              request.CoverContentType,
                request.CoverFileName,
                request.IsAvailable,
                request.IsReviewable,
                request.PublisherId, request.PublisherIdProvided,

               request.PersonFilters,
                request.ThemeIds
            );

            await _mediator.Send(command, cancellationToken);
            return NoContent();

        }

        [Authorize(Roles = "admin, moderator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(long id, CancellationToken cancellationToken)
        {

            var command = new DeleteBookCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();

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
            if (mode && (userId == 0 || roleClaim != "admin"))
            {
                return BadRequest();
            }
            var metadata = await _mediator.Send(new GetBookMetadataQuery(bookId, userId, mode));
            if (metadata != null)
                return Ok(metadata);
            return NotFound();
        }

        [HttpGet("{id}/contents")]
        public async Task<ActionResult<List<ContentDto>>> GetBookContents(long id, CancellationToken cancellationToken)
        {
            var query = new GetBookContentsQuery(id);
            var contents = await _mediator.Send(query, cancellationToken);
            return Ok(contents);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("{bookId}/contents/{contentId}")]

        public async Task<ActionResult> LinkContentToBook(long bookId, long contentId,
            [FromBody] BookContentLinkInputModel request, CancellationToken cancellationToken)
        {
            if (request.BookId != bookId || request.ContentId != contentId)
                return BadRequest(new { message = "ID в пути и теле запроса не совпадают" });

            var command = new LinkContentToBookCommand(bookId, contentId);
            await _mediator.Send(command, cancellationToken);
            return NoContent();

        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{bookId}/contents/{contentId}")]
        public async Task<ActionResult> UnlinkContentFromBook(long bookId, long contentId,
            CancellationToken cancellationToken)
        {

            var command = new UnlinkContentFromBookCommand(bookId, contentId);
            await _mediator.Send(command, cancellationToken);
            return NoContent();

        }

        [HttpGet("files/{bookFileId}/toc")]
        public async Task<ActionResult> GetToc(long bookFileId)
        {

            var json = await _mediator.Send(new GetTocQuery(bookFileId));
            if (json is null)
                return NotFound(new { message = "TOC не найден" });

            return Content(json, "application/json; charset=utf-8");

        }

        [HttpGet("files/{bookFileId}/chunks/{chunkIndex}")] //:инт можно ещё писать
        public async Task<ActionResult> GetChunk(long bookFileId, string chunkIndex)
        {

            var json = await _mediator.Send(new GetChunkQuery(bookFileId, chunkIndex));
            if (json is null)
                return NotFound(new { message = $"Фрагмент {chunkIndex} не найден" });

            return Content(json, "application/json; charset=utf-8");

        }
    }
}
