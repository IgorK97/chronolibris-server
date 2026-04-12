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
    {
        public ModerationTasksRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ModerationTask?> GetActiveByTarget(long TargetId, long TargetTypeId)
        {
            return await _context.ModerationTasks.AsNoTracking().Where(t =>
            t.TargetId == TargetId &&
            t.TargetTypeId == TargetTypeId &&
            t.StatusId == 2
            ).FirstOrDefaultAsync();
        }
    }
}
