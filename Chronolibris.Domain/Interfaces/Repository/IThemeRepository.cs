using Chronolibris.Domain.Entities;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IThemeRepository
    {
        Task<Theme?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<List<Theme>> GetByParentIdAsync(long? parentThemeId, CancellationToken cancellationToken = default);
        Task<List<Theme>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task AddAsync(Theme theme, CancellationToken cancellationToken = default);
        void Update(Theme theme);
        void Delete(Theme theme);
        Task<int> GetSubThemesCountAsync(long themeId, CancellationToken cancellationToken = default);
        Task<bool> HasSubThemesAsync(long themeId, CancellationToken cancellationToken = default);
        Task<bool> IsAncestorAsync(long potentialAncestorId, long? startParentId, CancellationToken ct);
    }
}