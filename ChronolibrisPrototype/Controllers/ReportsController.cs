using System.Security.Claims;
using Chronolibris.Application.Reports.Commands;
using Chronolibris.Application.Reports.Queries;
using Chronolibris.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChronolibrisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class CreateReportRequest
        {
            public long TargetId { get; set; }
            public long TargetTypeId { get; set; }
            public long ReasonTypeId { get; set; }
            public string? Description { get; set; }
        }

        [HttpPost]
        [Authorize(Roles = "reader")]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _mediator.Send(new CreateReportCommand(
                request.TargetId,
                request.TargetTypeId,
                request.ReasonTypeId,
                request.Description,
                userId));

           

            return Ok(result);
        }

        [HttpGet("reports")]
        public async Task<ActionResult<GetReportsResponse>> GetReports(
            [FromQuery] GetReportsRequest request)
        {
            var result = await _mediator.Send(new GetReportsQuery(
                request.LastTargetId,
                request.LastTargetTypeId,
                request.LastReportTypeId,
                request.Count,
                request.TargetTypeFilter,
                request.ReportTypeFilter,
                request.ReportStatusFilter,
                request.ReportStatusId,
                request.LastDate));

            return Ok(result);
        }

        [HttpGet("targets/{targetTypeId:long}/{targetId:long}")]
        public async Task<ActionResult<GetTargetInfoResponse>>
            GetTargetInfo(long targetTypeId, long targetId)
        {
            var result = await _mediator.Send(new GetTargetInfoQuery(targetId, targetTypeId));
            return Ok(result);
        }

        [HttpGet("reports/target")]
        public async Task<ActionResult<GetTargetReportsResponse>> GetTargetReports(
            [FromQuery] GetTargetReportsRequest request)
        {
            var result = await _mediator.Send(new GetTargetReportsQuery(
                request.TargetId,
                request.TargetTypeId,
                request.ReasonTypeId,
                request.Count,
                request.LastReportId));

            return Ok(result);
        }

        [HttpPost("tasks")]
        public async Task<ActionResult<CreateModerationTaskResponse>> CreateTask(
            [FromBody] CreateModerationTaskRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var moderatorId))
                return Unauthorized();

            var result = await _mediator.Send(new CreateModerationTaskCommand(
                request.TargetId,
                request.TargetTypeId,
                request.ReportTypeId,
                moderatorId));

            return Ok(result);
        }

        [HttpPut("tasks/{id:long}/resolution")]
        public async Task<ActionResult<TaskResolutionResponse>> ResolveTask(
            long id, [FromBody] TaskResolutionRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var moderatorId))
                return Unauthorized();

            var result = await _mediator.Send(new ResolveTaskCommand(id, request.Resolution, moderatorId, request.Comment));

            return Ok(result);
        }

    }
}
