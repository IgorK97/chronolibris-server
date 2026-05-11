using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using Chronolibris.Infrastructure.Data;
using Chronolibris.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
                ?? throw new ChronolibrisException(
                    $"Файл книги {bookFileId} не найден", ErrorType.NotFound);

            var fragments = result.PartFiles
                .Where(f => f.FileType == StoredFileType.Part)
                .Select((part, index) => new BookFragment
                {
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

            bookFile.StatusId = BookFileStatuses.COMPLETED;
            bookFile.CompletedAt = result.CompletedAt;

            await _context.SaveChangesAsync(ct);
        }

        public override async Task AddAsync(BookFile entity, CancellationToken token)
        {
            try
            {
                await _context.BookFiles.AddAsync(entity, token);
                await _context.SaveChangesAsync(token);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg)
            {
                if(pg.SqlState=="23505")
                    throw new ChronolibrisException(
                    "Файл такого формата уже существует для этой книги",
                    ErrorType.Conflict);
                if(pg.SqlState=="P0001")
                    throw new ChronolibrisException(pg.Message, ErrorType.Conflict);
                throw;
            }
        }

        public async Task<List<BookFile>> GetByBookIdAsync(long bookId, bool adminMode, CancellationToken cancellationToken = default)
        {
            var query = _context.BookFiles
                .Include(bf => bf.Format)
                .Include(bf => bf.BookFileStatus)
                .Where(bf => bf.BookId == bookId);

            if (!adminMode)
                query = query.Where(bf => bf.StatusId == 4);

            return await query.OrderBy(bf => bf.FormatId).ToListAsync(cancellationToken);
                //.OrderBy(bf => bf.FormatId)
                //.ToListAsync(cancellationToken);
        }

        public async Task<BookFile?> GetByBookIdAndFormatIdAsync(long bookId, int formatId, CancellationToken cancellationToken = default)
        {
            return await _context.BookFiles
                .FirstOrDefaultAsync(bf => bf.BookId == bookId && bf.FormatId == formatId, cancellationToken);
        }
    }
}