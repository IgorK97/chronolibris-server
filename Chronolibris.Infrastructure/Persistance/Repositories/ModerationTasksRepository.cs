using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Infrastructure.Data;
using Chronolibris.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    public class ModerationTasksRepository : GenericRepository<ModerationTask>, IModerationTasksRepository //Почему именно такой порядок
        //нашел ответ - базовый класс один, сначала он, потом уже все интерфейсы
    {
        public ModerationTasksRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ModerationTask?> GetLastTaskAsync(long targetId, long targetTypeId, CancellationToken token)
        {
            return await _context.ModerationTasks
                .Where(t => t.TargetId == targetId && t.TargetTypeId == targetTypeId)
                .OrderByDescending(t => t.StartedAt)
                .FirstOrDefaultAsync(token);
        }

        public async Task<ModerationTask?> GetActiveByTarget(long TargetId, long TargetTypeId, CancellationToken token = default)
        {
            return await _context.ModerationTasks.AsNoTracking().Where(t =>
            t.TargetId == TargetId &&
            t.TargetTypeId == TargetTypeId &&
            t.StatusId == 2
            ).FirstOrDefaultAsync(token);
        }

        public async Task<long?> TryCreateActiveTaskAsync(ModerationTask task, CancellationToken token)
        {
            var sql = @"
                INSERT INTO moderation_tasks 
                    (target_id, target_type_id, moderated_by, started_at, status_id, comment, check_number, reason_type_id)
                VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})
                ON CONFLICT (target_id, target_type_id, reason_type_id) WHERE status_id = 2 
                DO NOTHING
                RETURNING id;";

            var result = await _context.Database
                .SqlQueryRaw<long>(sql,
                    task.TargetId, task.TargetTypeId, task.ModeratedBy,
                    task.StartedAt, task.StatusId, task.Comment,
                    task.CheckNumber, task.ReasonTypeId)
                .ToListAsync(token);

            return result.FirstOrDefault();
        }
    }
}
