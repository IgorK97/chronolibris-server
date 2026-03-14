using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces
{
    // Application/Interfaces/IBookFileRepository.cs
    public interface IBookFileRepository : IGenericRepository<BookFile>
    {
        /// <summary>
        /// Сохраняет результат конвертации — фрагменты и изображения,
        /// меняет статус на Completed, проставляет CompletedAt.
        /// </summary>
        Task SaveConversionResultAsync(long bookFileId, ConversionResult result,
            CancellationToken ct = default);

        /// <summary>
        /// Меняет статус на Failed, записывает сообщение об ошибке.
        /// </summary>
        Task SetErrorAsync(long bookFileId, string errorMessage,
            CancellationToken ct = default);

        /// <summary>
        /// Меняет статус записи.
        /// </summary>
        Task SetStatusAsync(long bookFileId, int status,
            CancellationToken ct = default);
    }
}
