using Chronolibris.Domain.Entities;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IModerationTasksRepository : IGenericRepository<ModerationTask>
    {
        Task<long?> TryCreateActiveTaskAsync(ModerationTask task, CancellationToken token);
        Task<ModerationTask?> GetLastTaskAsync(long targetId, long targetTypeId, CancellationToken token);
        Task<ModerationTask?> GetActiveByTarget(long TargetId, long TargetTypeId, CancellationToken token);
    }

}
