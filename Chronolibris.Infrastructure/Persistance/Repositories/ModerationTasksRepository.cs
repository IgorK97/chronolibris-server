using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Infrastructure.Data;
using Chronolibris.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    public class ModerationTasksRepository : GenericRepository<ModerationTask>, IModerationTasksRepository
    {
        public ModerationTasksRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ModerationTask?> GetLastTaskAsync(long targetId, long targetTypeId, CancellationToken token)
        {

            IQueryable<ModerationTask> query = _context.ModerationTasks;
            if(targetTypeId==1) //книга
            {
                query = query.Where(t => t.BookId == targetId);
            }
            else if(targetTypeId == 3) //комментарий
            {
                query = query.Where(t => t.CommentId == targetId);
            }
            else if(targetTypeId == 2) //отзыв
            {
                query = query.Where(t => t.ReviewId == targetId);
            }
            return await query
                .OrderByDescending(t => t.StartedAt)
                .FirstOrDefaultAsync(token);
        }

        public async Task<ModerationTask?> GetActiveByTarget(long TargetId, long TargetTypeId, CancellationToken token = default)
        {
            IQueryable<ModerationTask> query = _context.ModerationTasks.AsNoTracking();
            if (TargetTypeId == 1) //книга
            {
                query = query.Where(t => t.BookId == TargetId);
            }
            else if (TargetTypeId == 3) //комментарий
            {
                query = query.Where(t => t.CommentId == TargetId);
            }
            else if (TargetTypeId == 2) //отзыв
            {
                query = query.Where(t => t.ReviewId == TargetId);
            }
            return await query.Where(t =>t.StatusId == 2).FirstOrDefaultAsync(token);
        }

        public async Task<long?> TryCreateActiveTaskAsync(ModerationTask task, CancellationToken token) //написал так, тогда, если такая запись уже есть,
            //то будет не исключение, а просто нулл вернет
        {
            string sql;
            List<long?> result;

            if (task.BookId != null)
            {
                sql = @"
                    INSERT INTO moderation_tasks 
                        (book_id, moderated_by, started_at, status_id, comment_text)
                    VALUES ({0}, {1}, {2}, {3}, {4})
                    ON CONFLICT (book_id) WHERE status_id = 2 
                    DO NOTHING
                    RETURNING id;";
                result = await _context.Database
                            .SqlQueryRaw<long?>(sql,
                                task.BookId, task.ModeratedBy,
                                task.StartedAt, task.StatusId, task.CommentText)
                            .ToListAsync(token);
            }
            else if (task.ReviewId != null)
            { 
                sql = @"
                    INSERT INTO moderation_tasks 
                        (review_id, moderated_by, started_at, status_id, comment_text)
                    VALUES ({0}, {1}, {2}, {3}, {4})
                    ON CONFLICT (review_id) WHERE status_id = 2 
                    DO NOTHING
                    RETURNING id;";
                result = await _context.Database
                 .SqlQueryRaw<long?>(sql,
                     task.ReviewId, task.ModeratedBy,
                     task.StartedAt, task.StatusId, task.CommentText)
                 .ToListAsync(token);
            }
            else if (task.CommentId != null)
            { 
                sql = @"
                    INSERT INTO moderation_tasks 
                        (comment_id, moderated_by, started_at, status_id, comment_text)
                    VALUES ({0}, {1}, {2}, {3}, {4})
                    ON CONFLICT (comment_id) WHERE status_id = 2 
                    DO NOTHING
                    RETURNING id;";
                result = await _context.Database
                            .SqlQueryRaw<long?>(sql,
                                task.CommentId, task.ModeratedBy,
                                task.StartedAt, task.StatusId, task.CommentText)
                            .ToListAsync(token);
            }
            else throw new ChronolibrisException("Неверный тип контента", ErrorType.Validation);
            return result.FirstOrDefault();
        }
    }
}
