using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Chronolibris.Infrastructure.Persistance.Repositories
{

    public class BookmarkRepository : GenericRepository<Bookmark>, IBookmarkRepository
    {
        public BookmarkRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task AddAsync(Bookmark entity, CancellationToken token = default)
        {
            try
            {
                await _context.Bookmarks.AddAsync(entity, token);
                await _context.SaveChangesAsync(); //потом вынести отсюда и сделать глобальный обработчик - но тогда потребуется глобальный обрабочтик ошибок сделать корректным,
                //чтобы мог отделять по сущностям или ещё как-то
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                switch (pgEx.SqlState)
                {
                    case "23505":
                        throw new ChronolibrisException("Закладка с такой позицией уже существует", ErrorType.Conflict);
                    case "23503": //внешний ключ
                        throw new ChronolibrisException("Файл книги был удален", ErrorType.NotFound);
                    default:
                        throw;
                }
            }
        }
        public async Task<List<Bookmark>> GetAllForBookAndUserAsync(long bookId, long userId, CancellationToken token = default)
        {
            return await _context.Bookmarks.Where(b => b.BookFileId == bookId && b.UserId == userId)
                .ToListAsync(token);
        }

        public async Task<Bookmark?> GetConcreteBookmark(long bookId, long userId, string xpointer, CancellationToken token = default)
        {
            return await _context.Bookmarks.Where(b => b.BookFileId == bookId && b.UserId == userId && b.Xpointer == xpointer).FirstOrDefaultAsync(token);
        }

        public async Task<(List<Bookmark> Items, int TotalCount)> GetPagedForUserAsync(
            long userId,
            int number,
            int pageSize,
            string? searchQuery,
            CancellationToken token = default)
        {
            var query = _context.Bookmarks
                .Include(b => b.BookFile)
                    .ThenInclude(bf => bf.Book)
                .Include(b => b.BookFile)
                    .ThenInclude(bf => bf.Format)
                .Where(b => b.UserId == userId && b.BookFile.Book.IsAvailable
                && (b.BookFile.StatusId==BookFileStatuses.COMPLETED || b.BookFile.StatusId==BookFileStatuses.ARCHIVE));
            if (number > 0)
            {
                query = query.Where(b => b.Id < number);
            }
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var pattern = $"%{searchQuery.Trim()}%";
                query = query.Where(b =>
                    EF.Functions.ILike(b.Context, pattern) ||
                    (b.Note != null && EF.Functions.ILike(b.Note, pattern)));
            }

            var totalCount = await query.CountAsync(token);

            var items = await query
                .OrderByDescending(b => b.Id)
                .Take(pageSize)
                .ToListAsync(token);

            return (items, totalCount);
        }
    }
}
