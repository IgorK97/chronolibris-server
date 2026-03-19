using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces
{
    public interface IReportRepository : IGenericRepository<Report>
    {
        Task<List<ReportShortDto>> GetReports(long? LastTargetId, 
            long? LastTargetTypeId, long? LastReportTypeId,
            int Count, bool TargetTypeFilter, bool ReportTypeFilter, 
            bool ReportStatusFilter, long? ReportStatusId, DateTime? LastDate);

        Task<GetTargetInfoResponse?> GetTargetInfo(long TargetId, long targetTypeId);

        Task<List<ReportDto>> GetTargetReports(long TargetId, long TargetTypeId, long ReportTypeId,
            int Count, long? LastReportId);

        Task<Report?> GetLastUserReport(long UserId,
            long TargetTypeId, long TargetId,
            long ReasonTypeId);

        Task<ModerationTask> CreateModerationTaskWithReportsAsync(
            long TargetId, long TargetTypeId, long ReportTypeId,
            long ModeratorId,
            ITransaction transaction);

    }
}
