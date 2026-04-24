using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task AttachReportsToTaskAsync(long taskId, long targetId, long targetTypeId, long reportTypeId, CancellationToken token)
        {
            await _context.Reports
                .Where(r => r.TargetId == targetId &&
                            r.TargetTypeId == targetTypeId &&
                            r.ReasonTypeId == reportTypeId &&
                            r.ModerationTaskId == null)
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.ModerationTaskId, taskId), token);
        }

       

        public async Task<Report?> GetLastUserReport(long UserId, long TargetTypeId, long TargetId, long ReasonTypeId, CancellationToken token = default)
        {
            return await _context.Reports.AsNoTracking()
                .Where(r => r.CreatedBy == UserId
                && r.TargetId == TargetId
                && r.ReasonTypeId == ReasonTypeId).OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(token);
                
        }

        public async Task<List<ReportShortDto>> GetReports(long moderatorId, long? LastTargetId, 
            long? LastTargetTypeId, long? LastReportTypeId, 
            int Count, bool TargetTypeFilter, bool ReportTypeFilter,
            bool ReportStatusFilter,
            long? ReportStatusId, DateTime? LastDate)
        {
            IQueryable<Report> query = _context.Reports
                .AsNoTracking().Include(r=>r.ModerationTask);
            if(TargetTypeFilter && LastTargetTypeId is not null)
            {
                query = query.Where(r => r.TargetTypeId == LastTargetTypeId);
            }
            if(ReportTypeFilter && LastReportTypeId is not null)
            {
                query = query.Where(r => r.ReasonTypeId == LastReportTypeId);
            }
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
                .GroupBy(r => new { r.TargetId, r.TargetTypeId, r.ReasonTypeId })
                .Select(r=>new ReportShortDto
                {
                    //Здесь нужно наверное это убрать и написать просто каунт, нет?
                     Count = r.Count(),
                     FirstReportDate = r.Min(r => r.CreatedAt),
                     LastReportDate = r.Max(r => r.CreatedAt),
                     ModerationTaskId = r
                     .Select(r=>r.ModerationTaskId).FirstOrDefault(),
                    TargetId = r.Key.TargetId,
                    TargetTypeId = r.Key.TargetTypeId,
                    ReasonTypeId = r.Key.ReasonTypeId,
                    Comment = r
                     .Where(r => r.ModerationTaskId!=null).Select(r => r.ModerationTask.Comment).FirstOrDefault() ?? "",
                    //ReasonTypeId = r.Select(r=>r.ReasonTypeId).FirstOrDefault(),
                    //TargetId = r.Select(r=>r.TargetId).FirstOrDefault(),
                    //TargetTypeId = r.Select(r=>r.TargetTypeId).FirstOrDefault(),
                    //Как исправить разыменование пустой ссылки?
                    TaskCreatedAt = r.Select(r =>  (r.ModerationTask == null)? (DateTime?) null:r.ModerationTask.StartedAt).FirstOrDefault(),
                     TaskResolvedAt = r.Select(r=> (r.ModerationTask == null) ? (DateTime?)null : r.ModerationTask.ResolvedAt).FirstOrDefault(),
                     TaskStatusId = r.Select(r=> (r.ModerationTask == null) ? (long?)null : r.ModerationTask.StatusId).FirstOrDefault(),
                });

            if(LastDate != null)
            {
                queryGrouping = queryGrouping.Where(r => r.FirstReportDate >  LastDate);
            }

            var resultQuery = queryGrouping.OrderBy(r => r.FirstReportDate).Take(Count);
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
                }).FirstOrDefaultAsync();
            }
            else if (TargetTypeId == 2)
            {
                return await _context.Comments.Where(c => c.Id == TargetId).Select(c =>
                new GetTargetInfoResponse
                {
                    TargetId = c.Id,
                    BookDescription = null,
                    BookTitle = null,
                    TargetTypeId = TargetTypeId,
                    Text = c.Text,
                    ParentCommentText = c.ParentComment != null ? c.ParentComment.Text : null,
                    ReaderId = c.UserId,
                    BookId = c.BookId,
                }).FirstOrDefaultAsync();
            }
            else if (TargetTypeId == 3)
            {
                return await _context.Reviews.Where(c => c.Id == TargetId).Select(r =>
                new GetTargetInfoResponse
                {
                    TargetId = r.Id,
                    BookDescription = null,
                    BookTitle = null,
                    TargetTypeId = TargetTypeId,
                    Text = r.ReviewText,
                    ReaderId = r.UserId,
                    BookId= r.BookId,
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
