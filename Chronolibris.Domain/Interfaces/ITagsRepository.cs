using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces
{
    public interface ITagsRepository
    {
        Task<List<TagDetails>> GetRootTagsAsync(
           long? tagTypeId, string? searchTerm, long? lastId, int limit,
           CancellationToken ct);
        Task<List<TagDetails>> GetChildTagsAsync(long parentTagId,
            long? lastId, int limit, CancellationToken ct);
        Task<IReadOnlyList<TagType>> GetTagTypesAsync(CancellationToken ct);
        Task<List<TagDetails>> GetTagsAsync(long? tagTypeId,
    string? searchTerm,
    long? lastId,       // вместо page
    int limit,          // вместо pageSize
    CancellationToken ct);
        Task<int> GetTagsCountAsync(long? tagTypeId, string? searchTerm, CancellationToken ct);
        Task<long> CreateAsync(Tag tag, CancellationToken ct);
        Task<bool> DeleteAsync(long tagId, CancellationToken ct);
        Task<Tag?> GetByIdAsync(long id, CancellationToken ct);
    }
}
