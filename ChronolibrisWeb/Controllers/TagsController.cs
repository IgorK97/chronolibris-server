
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.References.Tags;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("types")]
        public async Task<List<TagType>> GetTagTypes()
        {
            return await _mediator.Send(new GetTagTypesQuery());
        }

        [HttpGet]
        public async Task<PagedResult<TagDetails>> GetTags(
            long? tagTypeId = null,
            string? searchTerm = null,
            long? lastId = null,
            int pageSize = 20)
        {
            return await _mediator.Send(new GetRootTagsQuery(tagTypeId, searchTerm, lastId, pageSize));
        }

        [HttpGet("{parentId}/children")]
        public async Task<PagedResult<TagDetails>> GetChildTags(
            long parentId,
            long? lastId = null,
            int pageSize = 20)
        {
            return await _mediator.Send(new GetChildTagsQuery(parentId, lastId, pageSize));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request)
        {

            if (request.ParentTagId.HasValue && !request.RelationTypeId.HasValue)
                return BadRequest("Не указан тип отношения между тегами");

            if (request.RelationTypeId.HasValue && !request.ParentTagId.HasValue)
                return BadRequest("Не указан родительский тег");

            var tagId = await _mediator.Send(request);
            return Ok(tagId);
        }

        [HttpDelete("{tagId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTag(long tagId)
        {
            var result = await _mediator.Send(new DeleteTagRequest(tagId));
            return NoContent();
        }
    }
}