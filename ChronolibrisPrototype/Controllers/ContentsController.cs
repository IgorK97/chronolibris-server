// File: ChronolibrisPrototype.Controllers.ContentsController.cs
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChronolibrisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получает список контентов с фильтрацией и пагинацией
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentListResponse))]
        public async Task<ActionResult<ContentListResponse>> GetContents(
            [FromQuery] ContentFilterRequest filter, CancellationToken cancellationToken)
        {
            var query = new GetContentsQuery(filter);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Получает контент по идентификатору
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ContentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContentDto>> GetContentById(long id, CancellationToken cancellationToken)
        {
            var query = new GetContentByIdQuery(id);
            var content = await _mediator.Send(query, cancellationToken);

            if (content == null)
                return NotFound(new { message = $"Контент с ID {id} не найден" });

            return Ok(content);
        }

        [HttpGet("{id}/tags")]
        public async Task<ActionResult<List<TagDetails>>> GetContentTags(long id, CancellationToken cancellationToken)
        {
            var query = new GetContentTagsQuery(id);
            var tags = await _mediator.Send(query, cancellationToken);
            return Ok(tags);
        }

        [HttpGet("tags/search")]
        public async Task<ActionResult<List<TagDetails>>> SearchTags(
        [FromQuery] string searchTerm,
        [FromQuery] long? tagTypeId = null,
        [FromQuery] int limit = 5,
        CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest(new { message = "Search term is required" });

            var query = new SearchTagsQuery(searchTerm, tagTypeId, Math.Min(limit, 10));
            var tags = await _mediator.Send(query, cancellationToken);
            return Ok(tags);
        }

        [Authorize]
        [HttpPost("{contentId}/tags/{tagId}")]
        public async Task<ActionResult> AddTagToContent(long contentId, long tagId, CancellationToken cancellationToken)
        {
            var command = new AddTagToContentCommand(contentId, tagId);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound(new { message = "Контент или тег не найден" });

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{contentId}/tags/{tagId}")]
        public async Task<ActionResult> RemoveTagFromContent(long contentId, long tagId, CancellationToken cancellationToken)
        {
            var command = new RemoveTagFromContentCommand(contentId, tagId);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound(new { message = "Связь не найдена" });

            return NoContent();
        }

        /// <summary>
        /// Получает список книг для контента
        /// </summary>
        [HttpGet("{id}/books")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BookDto>))]
        public async Task<ActionResult<List<BookDto>>> GetContentBooks(long id, CancellationToken cancellationToken)
        {
            var query = new GetContentBooksQuery(id);
            var books = await _mediator.Send(query, cancellationToken);
            return Ok(books);
        }

        /// <summary>
        /// Создает новый контент
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<long>> CreateContent(
            [FromBody] CreateContentRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Некорректные данные запроса", errors = ModelState });

            try
            {
                var command = new CreateContentCommand(
                    request.Title,
                    request.Description,
                    request.CountryId,
                    request.ContentTypeId,
                    request.LanguageId,
                    request.Year,
                    request.ParentContentId,
                    request.Position,
                    request.PersonIds,
                    request.ThemeIds
                );

                var id = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetContentById), new { id = id }, id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Обновляет существующий контент
        /// </summary>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateContent(long id,
            [FromBody] UpdateContentRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Некорректные данные запроса", errors = ModelState });

            if (id != request.Id)
                return BadRequest(new { message = "ID в пути и теле запроса не совпадают" });

            try
            {
                var command = new UpdateContentCommand(
                    id,
                    request.Title,
                    request.Description,
                    request.CountryId,
                    request.ContentTypeId,
                    request.LanguageId,
                    request.Year,
                    request.ParentContentId,
                    request.Position,
                    request.PersonIds,
                    request.ThemeIds
                );

                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Контент с ID {id} не найден" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Удаляет контент
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteContent(long id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteContentCommand(id);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Контент с ID {id} не найден" });
            }
        }

        /// <summary>
        /// Привязывает книгу к контенту
        /// </summary>
        [Authorize]
        [HttpPost("{contentId}/books/{bookId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> LinkBookToContent(long contentId, long bookId,
            [FromBody] BookContentLinkRequest request, CancellationToken cancellationToken)
        {
            if (request.ContentId != contentId || request.BookId != bookId)
                return BadRequest(new { message = "ID в пути и теле запроса не совпадают" });

            try
            {
                var command = new LinkBookToContentCommand(contentId, bookId, request.Order);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Отвязывает книгу от контента
        /// </summary>
        [Authorize]
        [HttpDelete("{contentId}/books/{bookId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UnlinkBookFromContent(long contentId, long bookId,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = new UnlinkBookFromContentCommand(contentId, bookId);
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