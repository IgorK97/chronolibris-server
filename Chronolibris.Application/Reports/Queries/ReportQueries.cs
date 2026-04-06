using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Reports.Queries
{
    public record GetReportsQuery(
        long moderatorId,
        long? LastTargetId,
        long? LastTargetTypeId,
        long? LastReportTypeId,
        int Count,
        bool TargetTypeFilter,
        bool ReportTypeFilter,
        bool ReportStatusFilter,
        long? ReportStatusId,
        DateTime? LastDate):IRequest<GetReportsResponse>;

    public record GetTargetInfoQuery(
        long TargetId,
        long TargetTypeId) : IRequest<GetTargetInfoResponse?>;

    public record GetTargetReportsQuery(
        long TargetId,
        long TargetTypeId,
        long ReasonTypeId,
        int Count,
        long? LastReportId):IRequest<GetTargetReportsResponse>;

    
}
