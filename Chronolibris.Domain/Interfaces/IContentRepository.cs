// File: Chronolibris.Domain.Interfaces.IContentRepository.cs
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Interfaces
{
    public interface IContentRepository
    {
        Task<Content?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Content>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(List<Content> Items, int TotalCount, string? NextCursor, string? PrevCursor)> GetWithFilterAsync(
            ContentFilterRequest filter, CancellationToken cancellationToken = default);
        Task AddAsync(Content content, CancellationToken cancellationToken = default);
        void Update(Content content);
        void Delete(Content content);
        Task<int> GetBooksCountAsync(long contentId, CancellationToken cancellationToken = default);
        Task<List<Book>> GetBooksByContentIdAsync(long contentId, CancellationToken cancellationToken = default);
        Task LinkContentToBookAsync(long contentId, long bookId, int order, CancellationToken cancellationToken = default);
        Task UnlinkContentFromBookAsync(long contentId, long bookId, CancellationToken cancellationToken = default);
        Task<List<string>> GetAuthorNamesByContentIdAsync(long contentId, CancellationToken cancellationToken = default);
        Task<List<Theme>> GetThemesByContentIdAsync(long contentId, CancellationToken cancellationToken = default);

        Task<List<TagDetails>> GetTagsAsync(long contentId, CancellationToken ct);
        Task<List<TagDetails>> SearchTagsAsync(string searchTerm, long? tagTypeId, int limit, CancellationToken ct);
        Task<bool> AddTagAsync(long contentId, long tagId, CancellationToken ct);
        Task<bool> RemoveTagAsync(long contentId, long tagId, CancellationToken ct);
    }
}