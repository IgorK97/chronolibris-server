using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IBookFileRepository : IGenericRepository<BookFile>
    {
        Task SaveConversionResultAsync(long bookFileId, ConversionResult result,
            CancellationToken ct = default);
        Task<List<BookFile>> GetByBookIdAsync(long bookId, bool adminMode, CancellationToken cancellationToken = default);
        Task<BookFile?> GetByBookIdAndFormatIdAsync(long bookId, int formatId, CancellationToken cancellationToken = default);
    }
}
