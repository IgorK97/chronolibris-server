using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Reports
{
    public class GetTargetInfoQueryHandler :IRequestHandler<
        GetTargetInfoQuery, GetTargetInfoResponse?>
    {
        private readonly IReportRepository _reports;

        public GetTargetInfoQueryHandler( IReportRepository reports )
            => _reports = reports;

        public Task<GetTargetInfoResponse?> Handle(
            GetTargetInfoQuery request, CancellationToken token)
        {
            return _reports.GetTargetInfo(request.TargetId, request.TargetTypeId);
        }
    }
}
