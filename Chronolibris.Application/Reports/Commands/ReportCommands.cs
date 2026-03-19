using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Reports.Commands
{
    public record CreateReportCommand(
         long TargetId,
         long TargetTypeId,
         long ReasonTypeId,
         string Description,
         long UserId) : IRequest<CreateReportResult>;

    public record CreateReportResult(bool Success, string? Error);

    public record CreateModerationTaskCommand(
        long TargetId,
        long TargetTypeId,
        long ReportTypeId,
        long ModeratorId) : IRequest<CreateModerationTaskResponse>;

    public record ResolveTaskCommand(
        long TaskId,
        bool Resolution,
        long ModeratorId):IRequest<TaskResolutionResponse>;

}
