using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface ITagsRepository
    {
        Task<List<TagDetails>> GetRootTagsAsync(
           long? tagTypeId, string? searchTerm, long? lastId, int limit,
           CancellationToken ct);
        Task<List<TagDetails>> GetChildTagsAsync(long parentTagId,
            long? lastId, int limit, CancellationToken ct);
        Task<List<TagType>> GetTagTypesAsync(CancellationToken ct);
        Task<long> CreateAsync(Tag tag, CancellationToken ct);
        Task<bool> DeleteAsync(long tagId, CancellationToken ct);
    }
}
