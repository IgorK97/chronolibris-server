using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chronolibris.Infrastructure.Persistence.Repositories
{
    public class ThemeRepository : IThemeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Theme> _set;

        public ThemeRepository(ApplicationDbContext context)
        {
            _context = context;
            _set = context.Set<Theme>();
        }

        public async Task<bool> IsAncestorAsync(long potentialAncestorId, long? startParentId, CancellationToken ct)
        {
            var currentId = startParentId;

            while (currentId.HasValue)
            {
                if (currentId.Value == potentialAncestorId)
                    return true;

                var parent = await _context.Themes
                    .AsNoTracking()
                    .Where(t => t.Id == currentId)
                    .Select(t => t.ParentThemeId)
                    .FirstOrDefaultAsync(ct);

                currentId = parent;
            }

            return false;
        }

        public async Task<Theme?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _set
                .Include(t => t.ParentTheme)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Theme>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _set
                .Include(t => t.ParentTheme)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Theme>> GetByParentIdAsync(long? parentThemeId, CancellationToken cancellationToken = default)
        {
            return await _set
                .Include(t => t.ParentTheme)
                .Where(t => t.ParentThemeId == parentThemeId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Theme theme, CancellationToken cancellationToken = default)
        {
            await _set.AddAsync(theme, cancellationToken);
        }

        public void Update(Theme theme)
        {
            _set.Update(theme);
        }

        public void Delete(Theme theme)
        {
            _set.Remove(theme);
        }

        public async Task<int> GetSubThemesCountAsync(long themeId, CancellationToken cancellationToken = default)
        {
            return await _set.CountAsync(t => t.ParentThemeId == themeId, cancellationToken);
        }

        public async Task<bool> HasSubThemesAsync(long themeId, CancellationToken cancellationToken = default)
        {
            return await _set.AnyAsync(t => t.ParentThemeId == themeId, cancellationToken);
        }

        public async Task<IReadOnlyList<Theme>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Themes.Where(t => EF.Functions.ILike(t.Name, $"%{name}%")).ToListAsync(cancellationToken);
        }
    }
}