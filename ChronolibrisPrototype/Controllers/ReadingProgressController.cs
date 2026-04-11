//using System.Security.Claims;
//using Chronolibris.Application.Requests;
//using Chronolibris.Domain.Models;
//using Chronolibris.Infrastructure.Data;
//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Namotion.Reflection;

//namespace ChronolibrisPrototype.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ReadingProgressController : ControllerBase
//    {
//        private readonly IMediator _mediator;
//        public ReadingProgressController(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        [HttpGet("{bookFileId:long}")]
//        public async Task<ActionResult<ReadingProgressDto>> Get(
//            long bookFileId)
//        {
//            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            if (!long.TryParse(userIdClaim, out var userId))
//                return Unauthorized();

//            var result = await _mediator.Send(new GetReadingProgressQuery(
//                userId, bookFileId));

//            return result is null ? NoContent() : Ok(result);

//        }

//        [HttpPost]
//        public async Task<ActionResult<ReadingProgressDto>> Upsert(
//            [FromBody] UpsertReadingProgressRequest request)
//        {
//            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            if (!long.TryParse(userIdClaim, out var userId))
//                return Unauthorized();

//            var command = new UpdateReadingProgressCommand
//            {
//                UserId = userId,
//                BookFileId = request.BookFileId,
//                Percentage = request.Percentage,
//                ParaIndex = request.ParaIndex
//            };
//            var result = await _mediator.Send(command);
//            return Ok(result);
//        }

//        public record UpsertReadingProgressRequest(
//            long BookFileId,
//            decimal Percentage,
//            int ParaIndex);
//    }
//}
