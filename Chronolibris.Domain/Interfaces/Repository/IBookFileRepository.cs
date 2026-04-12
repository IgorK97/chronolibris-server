using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IBookFileRepository : IGenericRepository<BookFile>
    {
        Task SaveConversionResultAsync(long bookFileId, ConversionResult result,
            CancellationToken ct = default);
        Task SetErrorAsync(long bookFileId, string errorMessage,
            CancellationToken ct = default);
        Task SetStatusAsync(long bookFileId, int status,
            CancellationToken ct = default);

        Task<List<BookFile>> GetByBookIdAsync(long bookId, CancellationToken cancellationToken = default);
        Task<BookFile?> GetByBookIdAndFormatIdAsync(long bookId, int formatId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
    }
}
