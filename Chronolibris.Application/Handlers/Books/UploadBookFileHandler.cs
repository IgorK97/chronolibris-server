using System.IO.Compression;
using System.Xml;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Books;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Interfaces.Services;
using MediatR;

namespace Chronolibris.Application.Handlers.Books
{
    public class UploadBookFileHandler : IRequestHandler<UploadBookFileCommand, long>
    {
        private readonly IStorageService _bookStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFb2Converter _converter;


        public UploadBookFileHandler(
            IStorageService bookStorage,
            IUnitOfWork unitOfWork,
            IFb2Converter converter
            )
        {
            _bookStorage = bookStorage;
            _unitOfWork = unitOfWork;
            _converter = converter;
        }

        public async Task<long> Handle(UploadBookFileCommand request, CancellationToken cancellationToken)
        {
            if (request.IsReadable && request.FormatId != 1 ||
                !request.IsReadable && request.FormatId == 1 || 
                request.FormatId>2 || request.FormatId<1)
                throw new ChronolibrisException("Неверно указан формат и режим использования книги", ErrorType.Validation);

            var available_extension = Path.GetExtension(request.FileName).ToLowerInvariant();
            if (available_extension != ".fb2" && available_extension != ".epub")
                throw new ChronolibrisException(
                    "Неподдерживаемый формат. Допустимые форматы: .fb2, .epub",
                    ErrorType.Validation);

            //var existingFile = await _bookFileRepository.GetByBookIdAndFormatIdAsync(
            //    request.BookId, request.FormatId, cancellationToken);
            //if (existingFile != null)
            //    throw new ChronolibrisException($"Файл формата {request.FormatId} уже существует для этой книги. " +
            //        $"Сначала удалите старый файл, чтобы загрузить новый такого же формата", ErrorType.Conflict);

            var bookFile = new BookFile
            {
                Id = 0,
                BookId = request.BookId,
                FormatId = request.FormatId,
                StorageUrl = "",
                OriginalSize = request.FileSizeBytes,
                IsReadable = request.IsReadable,
                CreatedAt = DateTime.UtcNow,
                //CreatedBy = request.CreatedBy,
                StatusId = BookFileStatuses.PENDING,
                HistoricalText = request.HistoricalText
            };

            await _unitOfWork.BookFiles.AddAsync(bookFile, cancellationToken); //сразу сохраняет сам, потом можно подправить, если что
            //если неуспех, то исключение выбрасывает метод уже здесь
            //await _unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
                using var buffer = new MemoryStream();

                await request.FileStream.CopyToAsync(buffer, cancellationToken);
                buffer.Position = 0;

                if (extension == ".epub")
                    ValidateEpub(buffer);
                else if (extension == ".fb2")
                    ValidateFb2(buffer);

                Stream storageStream;
                string storageExtension;
                long compressedSize;

                if (extension == ".fb2")
                {
                    var zipped = CompressFb2ToZip(buffer, request.FileName);
                    compressedSize = zipped.Length;
                    storageStream = zipped;
                    storageExtension = ".fb2.zip";
                }

                else
                {
                    compressedSize = buffer.Length; //EPUB и так архив
                    storageStream = buffer;
                    storageExtension = ".epub";
                }

                //var storageUrl = await _bookStorage.SaveBookSourceAsync(
                //    bookFile.Id.ToString(),
                //    extension,
                //    buffer,
                //    cancellationToken);

                using (storageStream)
                {
                    string storageUrl = await _bookStorage.SaveBookSourceAsync(
                        bookFile.Id.ToString(),
                        storageExtension,
                        storageStream,
                        cancellationToken);
                    bookFile.StorageUrl = storageUrl;
                }

                bookFile.StoredSize = compressedSize;
                bookFile.StatusId = request.IsReadable
                    ? BookFileStatuses.UPLOADED
                    : BookFileStatuses.COMPLETED;
                bookFile.CompletedAt = DateTime.UtcNow;

                _unitOfWork.BookFiles.Update(bookFile); //надеюсь, детачт нигде не вызывался, проверить и убрать потом
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                var bookFileId = bookFile.Id;

                if (request.IsReadable)
                //await _bookConversionService.ProcessAsync(bookFile.Id);
                {
                    buffer.Position = 0; //так как епаб не формат для читалки, исключения не будет
                    //но могло бы быть - потом посмотреть, как подправить


                    var result = await _converter.ConvertAsync(
                        buffer,
                        bookId: bookFile.Id,
                        options: new ConversionOptions { TargetPartSize = 80 }
                      );

                    await _unitOfWork.BookFiles.SaveConversionResultAsync(bookFileId, result);

                }  

                return bookFile.Id;
            }
            catch(Exception ex)
            {
                string message = "";
                try
                {
                    //_bookFileRepository.Delete(bookFile);
                    bookFile.StatusId = BookFileStatuses.FAILED;
                    bookFile.CompletedAt = DateTime.UtcNow;
                    await _unitOfWork.SaveChangesAsync(CancellationToken.None);
                }
                catch (Exception ex1)
                {
                    message = "Ошибка при изменении данных о файле после неудачной загрузки"; //можно, наверное, логиовать, но клиенту что вернуть еще подумать
                }
                throw new ChronolibrisException("Ошибка при создании файла: проблема с хранилищем файлов или в процессе конвертации. " + message+
                    ex.Message, ErrorType.ServerException);
            }
        }

        private static MemoryStream CompressFb2ToZip(Stream fb2Stream, string originalFileName)
        {
            var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
            {
                var entryName = Path.GetFileName(originalFileName);
                var entry = zip.CreateEntry(entryName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                fb2Stream.Position = 0;
                fb2Stream.CopyTo(entryStream);
            }
            ms.Position = 0;
            return ms;
        }

        private static void ValidateEpub(Stream stream)
        {
            try
            {
                using var zip = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);

                var mimetype = zip.GetEntry("mimetype")
                    ?? throw new ChronolibrisException("Не найден файл mimetype — невалидный EPUB", ErrorType.Validation);

                using var mt = new StreamReader(mimetype.Open());
                var mimeContent = mt.ReadToEnd().Trim();
                if (mimeContent != "application/epub+zip")
                    throw new ChronolibrisException($"Неверный MIME-тип EPUB: «{mimeContent}»", ErrorType.Validation);

                var container = zip.GetEntry("META-INF/container.xml")
                    ?? throw new ChronolibrisException("Не найден META-INF/container.xml — невалидный EPUB", ErrorType.Validation);
            }
            catch (Exception ex) when (ex is not ChronolibrisException)
            {
                throw new ChronolibrisException("Файл не является ZIP-архивом — невалидный EPUB", ErrorType.Validation);
            }
            finally
            {
                stream.Position = 0;
            }
        }

        private static void ValidateFb2(Stream stream)
        {
            try
            {
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Ignore,
                    XmlResolver = null,
                };

                using var reader = XmlReader.Create(stream, settings);

                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element) continue;

                    //Корневой тег должен быть FictionBook (с namespace или без)
                    if (!reader.LocalName.Equals("FictionBook", StringComparison.OrdinalIgnoreCase))
                        throw new ChronolibrisException(
                            $"Корневой тег «{reader.LocalName}» не соответствует формату FB2", ErrorType.Validation);

                    break;
                }
            }
            catch (Exception ex) when (ex is not ChronolibrisException)
            {
                throw new ChronolibrisException("Файл не является валидным XML — невалидный FB2", ErrorType.Validation);
            }
            finally
            {
                stream.Position = 0;
            }
        }
    }
}