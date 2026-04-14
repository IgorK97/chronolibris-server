using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Contents;
using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Models;
using ChronolibrisWeb.InputModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisWeb.Controllers
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

        [HttpGet]
        public async Task<PagedResult<ContentDto>> GetContents(
            [FromQuery] ContentFilterRequest filter, CancellationToken cancellationToken)
        {
            var query = new GetContentsQuery(filter);
            var result = await _mediator.Send(query, cancellationToken);
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ContentDto?> GetContentById(long id, CancellationToken cancellationToken)
        {
            var query = new GetContentByIdQuery(id);
            var content = await _mediator.Send(query, cancellationToken);

            return content;
        }

        [HttpGet("{id}/books")]
        public async Task<ActionResult<List<BookDto>>> GetContentBooks(long id, CancellationToken cancellationToken)
        {
            var query = new GetContentBooksQuery(id);
            var books = await _mediator.Send(query, cancellationToken);
            return Ok(books);
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
            var query = new GetTagsQuery(searchTerm, tagTypeId, Math.Min(limit, 10));
            var tags = await _mediator.Send(query, cancellationToken);
            return Ok(tags);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("{contentId}/tags/{tagId}")]
        public async Task<ActionResult> AddTagToContent(long contentId, long tagId, CancellationToken cancellationToken)
        {
            var command = new AddTagToContentCommand(contentId, tagId);
            var result = await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{contentId}/tags/{tagId}")]
        public async Task<ActionResult> RemoveTagFromContent(long contentId, long tagId, CancellationToken cancellationToken)
        {
            var command = new RemoveTagFromContentCommand(contentId, tagId);
            var result = await _mediator.Send(command, cancellationToken);

            return NoContent();
        }



        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<long>> CreateContent(
            [FromBody] CreateContentRequest request, CancellationToken cancellationToken)
        {

            var command = new CreateContentCommand(
                request.Title,
                request.Description,
                request.CountryId,
                request.ContentTypeId,
                request.LanguageId,
                request.Year,
                request.PersonFilters,
                request.ThemeIds
            );

            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);

        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateContent(long id,
            [FromBody] UpdateContentRequest request, CancellationToken cancellationToken)
        {

            await _mediator.Send(request, cancellationToken);
            return NoContent();

        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteContent(long id, CancellationToken cancellationToken)
        {

            var command = new DeleteContentCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();

        }

        [Authorize(Roles = "admin")]
        [HttpPost("{contentId}/books/{bookId}")]
        public async Task<ActionResult> LinkBookToContent(long contentId, long bookId,
            [FromBody] BookContentLinkInputModel request, CancellationToken cancellationToken)
        {
            if (request.ContentId != contentId || request.BookId != bookId)
                return BadRequest(new { message = "ID в пути и теле запроса не совпадают" });


            var command = new LinkBookToContentCommand(contentId, bookId);
            await _mediator.Send(command, cancellationToken);
            return NoContent();

        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{contentId}/books/{bookId}")]
        public async Task<ActionResult> UnlinkBookFromContent(long contentId, long bookId,
            CancellationToken cancellationToken)
        {

            var command = new UnlinkBookFromContentCommand(contentId, bookId);
            await _mediator.Send(command, cancellationToken);
            return NoContent();

        }
    }
}