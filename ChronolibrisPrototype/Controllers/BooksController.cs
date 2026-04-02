using System.Security.Claims;
using System.Threading;
using Chronolibris.Application.Commands;
using Chronolibris.Application.Models;
using Chronolibris.Application.Queries;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisPrototype.Controllers
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

        /// <summary>
        /// Получает список книг с фильтрацией и пагинацией
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<BookListResponse>> GetBooks(
            [FromQuery] BookFilterRequest filter, CancellationToken cancellationToken)
        {
            var query = new GetBooksQuery(filter);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Получает книгу по идентификатору
        /// </summary>
        [HttpGet("{id}")]

        public async Task<ActionResult<BookDto>> GetBookById(long id, CancellationToken cancellationToken)
        {
            var query = new GetBookByIdQuery(id);
            var book = await _mediator.Send(query, cancellationToken);

            if (book == null)
                return NotFound(new { message = $"Книга с ID {id} не найдена" });

            return Ok(book);
        }


        /// <summary>
        /// Создаёт книгу. Обложка передаётся как Base64 в теле JSON.
        /// Порядок: 1) запись в БД → получаем id,
        ///           2) декодируем Base64 и загружаем в MinIO: covers/{id}/cover.{ext},
        ///           3) обновляем coverPath в БД.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<long>> CreateBook(
            [FromBody] Chronolibris.Application.Requests.CreateBookRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new Chronolibris.Application.Commands.CreateBookCommand(
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
                return CreatedAtAction(nameof(GetBookById), new { id }, id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Обновляет книгу. Поля с null без флага *Provided не трогаются.
        /// Если CoverBase64 передан — файл перезаписывается в MinIO, путь не меняется.
        /// </summary>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBook(
            long id,
            [FromBody] Chronolibris.Application.Requests.UpdateBookRequest request,
            CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest(new { message = "ID в пути и теле запроса не совпадают" });

            try
            {
                var command = new Chronolibris.Application.Commands.UpdateBookCommand
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
                    request.PublisherId,request.PublisherIdProvided,
                   
                   request.PersonFilters,
                    request.ThemeIds
                );

                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Книга с ID {id} не найдена" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Удаляет книгу
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]

        public async Task<ActionResult> DeleteBook(long id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteBookCommand(id);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Книга с ID {id} не найдена" });
            }
        }

        [HttpGet("{bookId}/info")]
        public async Task<ActionResult> GetBookMetadata(long bookId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var metadata = await _mediator.Send(new GetBookMetadataQuery(bookId, userId));
            if(metadata != null)
                return Ok(metadata);
            return NotFound();
        }

        [HttpPost("{bookId}/progress")]
        public async Task<ActionResult> UpdateReadingProgress(UpdateReadingProgressCommand request)
        {
            var metadata = await _mediator.Send(request);
            return NoContent();
        }

        [HttpGet("files/{bookFileId}/toc")]
        public async Task<ActionResult> GetToc(long bookFileId)
        {
            try
            {
                var json = await _mediator.Send(new GetTocQuery(bookFileId));
                if (json is null)
                    return NotFound(new { message = "TOC не найден" });

                return Content(json, "application/json; charset=utf-8");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("files/{bookFileId}/chunks/{chunkIndex}")] //:инт можно ещё писать
        public async Task<ActionResult> GetChunk(long bookFileId, string chunkIndex)
        {
            try
            {
                var json = await _mediator.Send(new GetChunkQuery(bookFileId, chunkIndex));
                if (json is null)
                    return NotFound(new { message = $"Фрагмент {chunkIndex} не найден" });

                return Content(json, "application/json; charset=utf-8");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("readbooks")]
        public async Task<IActionResult> GetReadBooks(long userId, long? lastId, int limit=20)
        {

            var result = await _mediator.Send(new GetReadBooksQuery(userId, lastId, limit));
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query, long userId, long? lastId, int limit=20)
        {
            var result = await _mediator.Send(new SearchBooksQuery(userId, lastId, limit, query));
            return Ok(result);
        }

        /// <summary>
        /// Получает список контентов для книги
        /// </summary>
        [HttpGet("{id}/contents")]
        public async Task<ActionResult<List<ContentDto>>> GetBookContents(long id, CancellationToken cancellationToken)
        {
            var query = new GetBookContentsQuery(id);
            var contents = await _mediator.Send(query, cancellationToken);
            return Ok(contents);
        }

        /// <summary>
        /// Привязывает контент к книге
        /// </summary>
        [Authorize]
        [HttpPost("{bookId}/contents/{contentId}")]

        public async Task<ActionResult> LinkContentToBook(long bookId, long contentId,
            [FromBody] BookContentLinkRequest request, CancellationToken cancellationToken)
        {
            if (request.BookId != bookId || request.ContentId != contentId)
                return BadRequest(new { message = "ID в пути и теле запроса не совпадают" });

            try
            {
                var command = new LinkContentToBookCommand(bookId, contentId, request.Order);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Отвязывает контент от книги
        /// </summary>
        [Authorize]
        [HttpDelete("{bookId}/contents/{contentId}")]
        public async Task<ActionResult> UnlinkContentFromBook(long bookId, long contentId,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = new UnlinkContentFromBookCommand(bookId, contentId);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Связь не найдена" });
            }
        }




    }
}
