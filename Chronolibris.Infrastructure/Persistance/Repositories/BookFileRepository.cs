using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using Chronolibris.Infrastructure.Data;
using Chronolibris.Infrastructure.Persistance.Repositories;
using Hangfire.Processing;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    public class BookFileRepository : GenericRepository<BookFile>, IBookFileRepository
    {

        public BookFileRepository(ApplicationDbContext context) : base(context)
        {
        }
        public override async Task<BookFile?> GetByIdAsync(long id, CancellationToken token)
        {
            return await _context.BookFiles.Include(bf => bf.Book).Where(bf => bf.Id==id).FirstOrDefaultAsync();
        }
        public async Task SaveConversionResultAsync(
            long bookFileId,
            ConversionResult result,
            CancellationToken ct = default)
        {
            var bookFile = await _context.BookFiles
                .FirstOrDefaultAsync(f => f.Id == bookFileId, ct)
                ?? throw new InvalidOperationException(
                    $"BookFile {bookFileId} не найден");

            var fragments = result.PartFiles
                .Where(f => f.FileType == StoredFileType.Part)
                .Select((part, index) => new BookFragment
                {
                    // Id генерируется БД — если Id в сущности required,
                    // убедись что в конфигурации EF стоит ValueGeneratedOnAdd.
                    // Временное значение 0 будет заменено БД при SaveChanges.
                    Id = 0,
                    BookFileId = bookFileId,
                    Position = index,
                    StorageUrl = $"{part.BookId}/{part.FileName}",
                    StartPos = part.GlobalStart,
                    EndPos = part.GlobalEnd,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            if (fragments.Count > 0)
                await _context.BookFragments.AddRangeAsync(fragments, ct);

            bookFile.BookFileStatusId = BookFileStatuses.COMPLETED;
            bookFile.CompletedAt = result.CompletedAt.UtcDateTime;

            await _context.SaveChangesAsync(ct);
        }

        public async Task SetErrorAsync(
            long bookFileId,
            string errorMessage,
            CancellationToken ct = default)
        {
            // ExecuteUpdateAsync — без загрузки сущности в память,
            // один UPDATE-запрос к БД.
            var updated = await _context.BookFiles
                .Where(f => f.Id == bookFileId)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(f => f.BookFileStatusId, BookFileStatuses.FAILED),
                    ct);
        }

        public async Task SetStatusAsync(
            long bookFileId,
            int status,
            CancellationToken ct = default)
        {
            await _context.BookFiles
                .Where(f => f.Id == bookFileId)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(f => f.BookFileStatusId, status),
                    ct);
        }

        public async Task<List<BookFile>> GetByBookIdAsync(long bookId, CancellationToken cancellationToken = default)
        {
            return await _context.BookFiles
                .Include(bf => bf.Format)
                .Include(bf => bf.BookFileStatus)
                .Where(bf => bf.BookId == bookId)
                .OrderBy(bf => bf.FormatId)
                .ToListAsync(cancellationToken);
        }

        public async Task<BookFile?> GetByBookIdAndFormatIdAsync(long bookId, int formatId, CancellationToken cancellationToken = default)
        {
            return await _context.BookFiles
                .FirstOrDefaultAsync(bf => bf.BookId == bookId && bf.FormatId == formatId, cancellationToken);
        }

        //public async Task<BookFile> ExistsAvailableAsync(long id, CancellationToken cancellationToken = default)
        //{
        //    return await _context.BookFiles.AnyAsync(bf => bf.Id == id && bf.Book.IsAvailable, cancellationToken);
        //}
    }
}
