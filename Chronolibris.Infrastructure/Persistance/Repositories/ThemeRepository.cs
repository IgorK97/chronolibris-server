using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.Persistence.Repositories
{
    public class ThemeRepository : IThemeRepository
    {
        private readonly ApplicationDbContext _context;

        public ThemeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Theme?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Themes
                .Include(t => t.ParentTheme)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<List<Theme>> GetByParentIdAsync(long? parentThemeId, CancellationToken cancellationToken = default)
        {
            return await _context.Themes
                .Include(t => t.ParentTheme)
                .Where(t => t.ParentThemeId == parentThemeId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Theme theme, CancellationToken cancellationToken = default)
        {
            await _context.Themes.AddAsync(theme, cancellationToken);
        }

        public void Update(Theme theme)
        {
            _context.Themes.Update(theme);
        }

        public void Delete(Theme theme)
        {
            _context.Themes.Remove(theme);
        }

        public async Task<int> GetSubThemesCountAsync(long themeId, CancellationToken cancellationToken = default)
        {
            return await _context.Themes.CountAsync(t => t.ParentThemeId == themeId, cancellationToken);
        }

        public async Task<bool> HasSubThemesAsync(long themeId, CancellationToken cancellationToken = default)
        {
            return await _context.Themes.AnyAsync(t => t.ParentThemeId == themeId, cancellationToken);
        }

        public async Task<List<Theme>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Themes.Where(t => EF.Functions.ILike(t.Name, $"%{name}%")).ToListAsync(cancellationToken);
        }
    }
}