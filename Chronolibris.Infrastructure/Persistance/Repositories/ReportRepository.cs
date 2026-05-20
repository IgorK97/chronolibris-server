using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using Chronolibris.Infrastructure.Data;
using Chronolibris.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        public ReportRepository(ApplicationDbContext context) : base(context) { }

        public async Task AttachReportsToTaskAsync(long taskId, long targetId, long targetTypeId, CancellationToken token)
        {
            await _context.Reports.Include(r => r.ModerationTask)
                .Where(r => r.TargetId == targetId &&
                            r.TargetTypeId == targetTypeId &&
                            r.ModerationTaskId == null)
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.ModerationTaskId, taskId), token);
        }

        public async Task<Report?> GetLastUserReport(long UserId, long TargetTypeId, long TargetId, long ReasonTypeId, CancellationToken token = default)
        {
            return await _context.Reports.Include(r => r.ModerationTask)
                .Where(r => r.CreatedBy == UserId
                && r.TargetId == TargetId 
                && r.TargetTypeId == TargetTypeId
                && r.ReasonTypeId == ReasonTypeId 
                //&& (r.ModerationTaskId== null || 
                //r.ModerationTask!=null && r.ModerationTask.StatusId==2)
                ).OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(token);
                
        }

        public async Task<List<ReportShortDto>> GetReports(long moderatorId, long? LastTargetId, 
            long? LastTargetTypeId,
            int Count, bool TargetTypeFilter,
            bool ReportStatusFilter,
            long? ReportStatusId, DateTime? LastDate)
        {
            
            IQueryable<Report> query = _context.Reports
                .Include(r=>r.ModerationTask);
            if(TargetTypeFilter && LastTargetTypeId is not null)
            {
                query = query.Where(r => r.TargetTypeId == LastTargetTypeId);
            }
            //if(ReportTypeFilter && LastReportTypeId is not null)
            //{
            //    query = query.Where(r => r.ReasonTypeId == LastReportTypeId);
            //}
            if(ReportStatusFilter)
            {
                if (ReportStatusId is not null) {
                    if (moderatorId == 0)
                        throw new ChronolibrisException("Не указан модератор", ErrorType.Validation);
                    query = query.Where(r => r.ModerationTaskId != null && r.ModerationTask.StatusId == ReportStatusId && r.ModerationTask.ModeratedBy == moderatorId);
                }
                else
                {
                    query = query.Where(r => r.ModerationTaskId == null);
                }
            }
            var queryGrouping = query
                .GroupBy(r => new { r.TargetId, r.TargetTypeId })
                .Select(r=>new ReportShortDto
                {
                     Count = r.Count(),
                     FirstReportDate = r.Min(r => r.CreatedAt),
                     LastReportDate = r.Max(r => r.CreatedAt),
                     ModerationTaskId = r
                     .Select(r=>r.ModerationTaskId).FirstOrDefault(),
                    TargetId = r.Key.TargetId,
                    TargetTypeId = r.Key.TargetTypeId,
                    ReasonTypeIds = r.Select(r =>r.ReasonTypeId).Distinct().ToList(),
                    Comment = r
                     .Where(r => r.ModerationTaskId!=null).Select(r => r.ModerationTask.Comment).FirstOrDefault() ?? "",
                    //ReasonTypeId = r.Select(r=>r.ReasonTypeId).FirstOrDefault(),
                    //TargetId = r.Select(r=>r.TargetId).FirstOrDefault(),
                    //TargetTypeId = r.Select(r=>r.TargetTypeId).FirstOrDefault(),
                    TaskCreatedAt = r.Select(r =>  (r.ModerationTask == null)? (DateTime?) null:r.ModerationTask.StartedAt).FirstOrDefault(),
                     TaskResolvedAt = r.Select(r=> (r.ModerationTask == null) ? (DateTime?)null : r.ModerationTask.ResolvedAt).FirstOrDefault(),
                     TaskStatusId = r.Select(r=> (r.ModerationTask == null) ? (long?)null : r.ModerationTask.StatusId).FirstOrDefault(),
                });

            if(LastDate != null)
            {
                queryGrouping = queryGrouping.Where(r => r.FirstReportDate >  LastDate);
            }

            IQueryable<ReportShortDto>? resultQuery;

            if(ReportStatusId>=3)
                resultQuery = queryGrouping.OrderByDescending(r => r.FirstReportDate).Take(Count);
            else
                resultQuery = queryGrouping.OrderBy(r => r.FirstReportDate).Take(Count);
            return await resultQuery.ToListAsync();
           
        }

        public async Task<GetTargetInfoResponse?> GetTargetInfo(long TargetId, long TargetTypeId)
        {
            if (TargetTypeId == 1)
            {
                return await _context.Books.Where(b => b.Id == TargetId).Select(b =>
                new GetTargetInfoResponse
                {
                    TargetId = b.Id,
                    BookDescription = b.Description,
                    BookTitle = b.Title,
                    TargetTypeId = TargetTypeId,
                    Text = null,
                    ReaderId = null,
                    BookId = b.Id,
                    IsActive = b.IsAvailable,
                    LastUpdatedAt = b.UpdatedAt ?? b.CreatedAt,
                }).FirstOrDefaultAsync();
            }
            else if (TargetTypeId == 3)
            {
                return await _context.Comments.Where(c => c.Id == TargetId).Join(_context.Users, c => c.UserId, u => u.Id, (c, u) =>
                new GetTargetInfoResponse
                {
                    TargetId = c.Id,
                    BookDescription = null,
                    BookTitle = null,
                    TargetTypeId = TargetTypeId,
                    Text = c.Text,
                    ParentCommentText = c.ParentComment != null ? c.ParentComment.Text : null,
                    ReaderId = c.UserId,
                    ReaderName = u.UserName,
                    BookId = c.BookId,
                    IsActive = c.DeletedAt==null,
                    LastUpdatedAt = c.DeletedAt ?? c.CreatedAt,
                }).FirstOrDefaultAsync();
            }
            else if (TargetTypeId == 2)
            {
                return await _context.Reviews.Where(c => c.Id == TargetId).Join(_context.Users, r => r.UserId, u => u.Id, (r, u) =>
                new GetTargetInfoResponse
                {
                    TargetId = r.Id,
                    BookDescription = null,
                    BookTitle = null,
                    TargetTypeId = TargetTypeId,
                    Text = r.ReviewText,
                    ReaderId = r.UserId,
                    ReaderName = u.UserName,
                    BookId = r.BookId,
                    IsActive = r.DeletedAt == null,
                    LastUpdatedAt = r.DeletedAt ?? r.CreatedAt,
                }).FirstOrDefaultAsync();
            }
            return null;
        }

        public async Task<List<ReportDto>> GetTargetReports(long TargetId, long TargetTypeId, long ReportTypeId, int Count, long? LastReportId)
        {
            IQueryable<Report> query = _context.Reports
                .AsNoTracking().Where(r=>r.TargetId==TargetId && r.TargetTypeId==TargetTypeId
                && r.ReasonTypeId==ReportTypeId);

            if (LastReportId != null)
            {
                query = query.Where(r => r.Id > LastReportId);
            }

            return await query.OrderBy(r => r.Id)
                .Take(Count)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    CreatedAt = r.CreatedAt,
                    ReporterId = r.CreatedBy,
                    Text = r.Description
                }).ToListAsync();
        }
    }
}
