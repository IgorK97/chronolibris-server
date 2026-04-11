
using System.Security.Claims;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
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
            long? lastId = null,
            int pageSize = 20)
        {

            var items = await _tagsRepository.GetRootTagsAsync(
                tagTypeId, searchTerm, lastId, pageSize, CancellationToken.None);

            var hasNext = items.Count > pageSize;
            var result = items.Take(pageSize).ToList();

            return Ok(new PagedResult<TagDetails>
            {
                Items = result,
                Limit = pageSize,
                HasNext = hasNext,
                LastId = result.LastOrDefault()?.Id
            });

            //var result = await _mediator.Send(
            //    new GetTagsQuery(tagTypeId, searchTerm, 0, pageSize));
            //return Ok(result);
        }

        [HttpGet("{parentId}/children")]
        public async Task<IActionResult> GetChildTags(
            long parentId,
            long? lastId = null,
            int pageSize = 20)
        {
            var items = await _tagsRepository.GetChildTagsAsync(
                parentId, lastId, pageSize, CancellationToken.None);

            var hasNext = items.Count > pageSize;
            var result = items.Take(pageSize).ToList();

            return Ok(new PagedResult<TagDetails>
            {
                Items = result,
                Limit = pageSize,
                HasNext = hasNext,
                LastId = result.LastOrDefault()?.Id
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Name is required");

            // Если указан родитель — тип отношения обязателен
            if (request.ParentTagId.HasValue && !request.RelationTypeId.HasValue)
                return BadRequest("RelationTypeId is required when ParentTagId is specified");

            // Если тип отношения указан — родитель обязателен
            if (request.RelationTypeId.HasValue && !request.ParentTagId.HasValue)
                return BadRequest("ParentTagId is required when RelationTypeId is specified");

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