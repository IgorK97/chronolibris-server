
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

namespace ChronolibrisWeb.Controllers
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