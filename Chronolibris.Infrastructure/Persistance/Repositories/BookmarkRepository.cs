using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.Persistance.Repositories
{

    public class BookmarkRepository : GenericRepository<Bookmark>, IBookmarkRepository
    {
        public BookmarkRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<List<Bookmark>> GetAllForBookAndUserAsync(long bookId, long userId, CancellationToken token = default)
        {
            return await _context.Bookmarks.Where(b => b.BookFileId == bookId && b.UserId == userId)
                .ToListAsync(token);
        }

        public async Task<Bookmark?> GetConcreteBookmark(long bookId, long userId, int paraIndex, CancellationToken token = default)
        {
            return await _context.Bookmarks.Where(b => b.BookFileId == bookId && b.UserId == userId && b.ParaIndex == paraIndex).FirstOrDefaultAsync(token);
        }
    }
}
