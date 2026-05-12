using System.Security.Claims;
using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Models;
using ChronolibrisWeb.InputModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ChronolibrisWeb.Controllers
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


        [HttpPost]
        [Authorize(Roles = "reader")]
        [EnableRateLimiting("reports")]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportInputModel request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _mediator.Send(new CreateReportCommand(
                request.TargetId,
                request.TargetTypeId,
                request.ReasonTypeId,
                request.Description,
                userId), ct);
           

            return Ok(result);
        }

        [HttpGet("reports")]
        [Authorize(Roles ="admin, moderator")]
        public async Task<ActionResult<GetReportsResponse>> GetReports(
            [FromQuery] GetReportsRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var userId))
                userId = 0;

            var result = await _mediator.Send(new GetReportsQuery(
                userId,
                request.LastTargetId,
                request.LastTargetTypeId,
                //request.LastReportTypeId,
                request.Count,
                request.TargetTypeFilter,
                //request.ReportTypeFilter,
                request.ReportStatusFilter,
                request.ReportStatusId,
                request.LastDate));

            return Ok(result);
        }

        [HttpGet("targets/{targetTypeId}/{targetId}")]
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult<GetTargetInfoResponse>>
            GetTargetInfo(long targetTypeId, long targetId)
        {
            var result = await _mediator.Send(new GetTargetInfoQuery(targetId, targetTypeId));
            return Ok(result);
        }

        [HttpGet("reports/target")]
        [Authorize(Roles = "admin, moderator")]
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
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult<CreateModerationTaskResponse>> CreateTask(
            [FromBody] CreateModerationTaskRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out var moderatorId))
                return Unauthorized();

            var result = await _mediator.Send(new CreateModerationTaskCommand(
                request.TargetId,
                request.TargetTypeId,
                moderatorId));

            return Ok(result);
        }

        [HttpPut("tasks/{id}/resolution")]
        [Authorize(Roles = "admin, moderator")]
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
