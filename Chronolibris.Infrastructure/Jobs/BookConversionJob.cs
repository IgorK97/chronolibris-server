using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Fb2Converter;
using Chronolibris.Application.Fb2Converter.Interfaces;
using Chronolibris.Application.Jobs;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Interfaces.Services;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Chronolibris.Infrastructure.DataAccess.Jobs
{
    public sealed class BookConversionJob : IBookConversionJob
    {
        private readonly IFb2Converter _converter;
        private readonly IStorageService _storage;       
        private readonly IBookFileRepository _bookFiles;
        //private readonly ILogger<BookConversionJob> _log;

        public BookConversionJob(
            IFb2Converter converter,
            IStorageService storage,
            IBookFileRepository bookFiles
            //,ILogger<BookConversionJob> log
            )
        {
            _converter = converter;
            _storage = storage;
            _bookFiles = bookFiles;
            //_log = log;
        }

        //[AutomaticRetry(Attempts = 2, DelaysInSeconds = [60, 300])]
        [AutomaticRetry(Attempts =0)]
        public async Task ProcessAsync(long bookFileId)
        {
            //_log.LogInformation("Начало конвертации bookFileId={Id}", bookFileId);

            var bookFile = await _bookFiles.GetByIdAsync(bookFileId)
                ?? throw new InvalidOperationException($"BookFile {bookFileId} не найден");

            // Идемпотентность
            if (bookFile.BookFileStatusId != BookFileStatuses.UPLOADED)
            {
                //_log.LogWarning("bookFileId={Id} уже обработан, пропускаем", bookFileId);
                return;
            }

            try
            {
                // Получаем поток через абстракцию — IMinioClient нигде не виден
                //var fb2Stream = await _storage.ReadStreamAsync(bookFile.StorageUrl)
                //    ?? throw new InvalidOperationException(
                //        $"Объект {bookFile.StorageUrl} не найден в хранилище");

                var fb2Stream = await _storage.ReadByStorageUrlAsync(bookFile.StorageUrl)
    ?? throw new InvalidOperationException(
        $"Объект {bookFile.StorageUrl} не найден в хранилище");

                await using (fb2Stream)
                {
                    var result = await _converter.ConvertAsync(
                        fb2Stream,
                        bookId: bookFile.Id,
                        options: new ConversionOptions { TargetPartSize = 88 }
                      );

                    await _bookFiles.SaveConversionResultAsync(bookFileId, result);
                }

                //_log.LogInformation("Конвертация завершена bookFileId={Id}", bookFileId);
            }
            catch (Exception ex)
            {
                //_log.LogError(ex, "Ошибка конвертации bookFileId={Id}", bookFileId);
                await _bookFiles.SetErrorAsync(bookFileId, ex.Message);
                throw;
            }
        }
    }
}
