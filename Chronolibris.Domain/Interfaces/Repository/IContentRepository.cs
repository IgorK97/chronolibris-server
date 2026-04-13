using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IContentRepository
    {
        Task<Content?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<List<Content>> GetAllAsync(CancellationToken cancellationToken = default);
        //Task<(List<Content> Items, int TotalCount, long? NextCursor, long? PrevCursor)> GetWithFilterAsync(
        //    ContentFilterRequest filter, CancellationToken cancellationToken = default);
        Task<ContentDto?> GetDtoByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<List<BookDto>> GetBooksDtoByContentIdAsync(long contentId, CancellationToken cancellationToken = default);
        void SyncThemes(Content content, List<long> newThemeIds);

        void SyncParticipations(Content content, List<PersonRoleFilter> personFilters);

        Task<PagedResult<ContentDto>> GetWithFilterAsync(ContentFilterRequest filter, CancellationToken ct = default);

        Task AddAsync(Content content, CancellationToken cancellationToken = default);
        void Update(Content content);
        void Delete(Content content);
        //Task SyncThemesAsync(long contentId, List<long> newThemeIds, CancellationToken cancellationToken = default);
        //Task SyncPersonsAsync(long contentId, List<PersonRoleFilter> newPersons, CancellationToken cancellationToken = default);
        Task SyncTagsAsync(long contentId, List<long> TagIds, CancellationToken cancellationToken);
        Task<int> GetBooksCountAsync(long contentId, CancellationToken cancellationToken = default);
        Task<List<Book>> GetBooksByContentIdAsync(long contentId, CancellationToken cancellationToken = default);
        Task LinkContentToBookAsync(long contentId, long bookId, CancellationToken cancellationToken = default);
        Task UnlinkContentFromBookAsync(long contentId, long bookId, CancellationToken cancellationToken = default);
        Task<List<string>> GetAuthorNamesByContentIdAsync(long contentId, CancellationToken cancellationToken = default);
        Task<List<Theme>> GetThemesByContentIdAsync(long contentId, CancellationToken cancellationToken = default);

        Task<List<TagDetails>> GetTagsAsync(long contentId, CancellationToken ct);
        Task<List<TagDetails>> SearchTagsAsync(string searchTerm, long? tagTypeId, int limit, CancellationToken ct);
        Task<bool> AddTagAsync(long contentId, long tagId, CancellationToken ct);
        Task<bool> RemoveTagAsync(long contentId, long tagId, CancellationToken ct);
    }
}