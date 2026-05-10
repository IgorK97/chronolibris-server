using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IContentRepository
    {
        Task<Content?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<ContentDto?> GetDtoByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<List<BookDto>> GetBooksDtoByContentIdAsync(long contentId, CancellationToken cancellationToken = default);
        void SyncThemes(Content content, List<long> newThemeIds);
        void SyncParticipations(Content content, List<PersonRoleFilter> personFilters);
        Task<PagedResult<ContentDto>> GetWithFilterAsync(ContentFilterRequest filter, CancellationToken ct = default);
        Task AddAsync(Content content, CancellationToken cancellationToken = default);
        void Delete(Content content);
        Task LinkContentToBookAsync(long contentId, long bookId, CancellationToken cancellationToken = default);
        Task UnlinkContentFromBookAsync(long contentId, long bookId, CancellationToken cancellationToken = default);
        Task<List<TagDetails>> GetTagsAsync(long contentId, CancellationToken ct);
        Task<bool> AddTagAsync(long contentId, long tagId, CancellationToken ct);
        Task<bool> RemoveTagAsync(long contentId, long tagId, CancellationToken ct);
    }
}