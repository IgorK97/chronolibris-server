using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.Persistance.Repositories
{
    public class BookFileRepository : GenericRepository<BookFile>, IBookFileRepository
    {

        public BookFileRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task SaveConversionResultAsync(
            long bookFileId,
            ConversionResult result,
            CancellationToken ct = default)
        {
            var bookFile = await _context.BookFiles
                .FirstOrDefaultAsync(f => f.Id == bookFileId, ct)
                ?? throw new InvalidOperationException(
                    $"BookFile {bookFileId} не найден");

            // Создаём фрагменты только из Part-файлов (не toc)
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

            // Обновляем саму запись файла
            bookFile.BookFileStatusId = BookFileStatuses.COMPLETED;
            bookFile.CompletedAt = result.CompletedAt.UtcDateTime;

            await _context.SaveChangesAsync(ct);
        }

        /// <inheritdoc/>
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

            // Опционально: логировать если запись не найдена
            // if (updated == 0) _log.LogWarning(...)
        }

        /// <inheritdoc/>
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
    }
}
