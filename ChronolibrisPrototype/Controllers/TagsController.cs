// Controllers/TagsController.cs
using System.Security.Claims;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITagsRepository _tagsRepository;

        public TagsController(IMediator mediator, ITagsRepository tagsRepository)
        {
            _mediator = mediator;
            _tagsRepository = tagsRepository;
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetTagTypes()
        {
            var types = await _tagsRepository.GetTagTypesAsync(CancellationToken.None);
            return Ok(types);
        }

        [HttpGet]
        public async Task<IActionResult> GetTags(
            long? tagTypeId = null,
            string? searchTerm = null,
            int page = 0,
            int pageSize = 20)
        {
            var result = await _mediator.Send(
                new GetTagsQuery(tagTypeId, searchTerm, 0, pageSize));
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Name is required");

            var tagId = await _mediator.Send(request);
            return CreatedAtAction(nameof(GetTags), new { }, tagId);
        }

        [HttpDelete("{tagId}")]
        [Authorize]
        public async Task<IActionResult> DeleteTag(long tagId)
        {
            var result = await _mediator.Send(new DeleteTagRequest(tagId));
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}