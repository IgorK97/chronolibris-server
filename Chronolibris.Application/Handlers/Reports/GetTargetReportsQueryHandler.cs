using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Reports
{
    public class GetTargetReportsQueryHandler :IRequestHandler
        <GetTargetReportsQuery, GetTargetReportsResponse>
    {
        private readonly IReportRepository _reports;

        public GetTargetReportsQueryHandler(IReportRepository reportRepository)
            => _reports = reportRepository;

        public async Task<GetTargetReportsResponse> Handle
            (GetTargetReportsQuery request, CancellationToken cancellationToken)
        {
            var items = await _reports.GetTargetReports(
                request.TargetId,
                request.TargetTypeId,
                request.ReasonTypeId,
                request.Count + 1,
                request.LastReportId);

            var hasNext = items.Count > request.Count;
            if (hasNext)
                items = items.Take(request.Count).ToList();

            return new GetTargetReportsResponse
            {
                Reports = items,
                HasNext = hasNext,
                Count = items.Count,
                LastReportId = items.LastOrDefault()?.Id ?? 0,
            };
        }
    }
}
