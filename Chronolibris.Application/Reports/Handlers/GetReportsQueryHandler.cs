using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Reports.Queries;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Reports.Handlers
{//another constructor type
    public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery,
        GetReportsResponse>
    {
        private readonly IReportRepository _reports;

        public GetReportsQueryHandler(IReportRepository reports)
            => _reports = reports;

        public async Task<GetReportsResponse> Handle(
            GetReportsQuery request, CancellationToken token)
        {
            var items = await _reports.GetReports(request.moderatorId, request.LastTargetId,
                request.LastTargetTypeId,
                request.LastReportTypeId,
                request.Count + 1,
                request.TargetTypeFilter,
                request.ReportTypeFilter,
                request.ReportStatusFilter,
                request.ReportStatusId,
                request.LastDate);

            var hasNext = items.Count > request.Count;

            if (hasNext)
            {
                items = items.Take(request.Count).ToList();
            }

            var last = items.LastOrDefault();

            return new GetReportsResponse
            {
                Reports = items,
                HasNext = hasNext,
                Count = items.Count,
                LastTargetId = last?.TargetId ?? 0,
                LastTargetTypeId = last?.TargetTypeId ?? 0,
                LastReportTypeId = last?.ReasonTypeId ?? 0,
            };
        }
    }

}