using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class GetBookFilesHandler : IRequestHandler<GetBookFilesQuery, List<BookFileDto>>
    {
        private readonly IBookFileRepository _bookFileRepository;

        public GetBookFilesHandler(IBookFileRepository bookFileRepository)
        {
            _bookFileRepository = bookFileRepository;
        }

        public async Task<List<BookFileDto>> Handle(GetBookFilesQuery request, CancellationToken cancellationToken)
        {
            var bookFiles = await _bookFileRepository.GetByBookIdAsync(request.BookId, cancellationToken);

            return bookFiles.Select(bf => new BookFileDto
            {
                Id = bf.Id,
                BookId = bf.BookId,
                FormatId = bf.FormatId,
                FormatName = bf.Format?.Name,
                StorageUrl = bf.StorageUrl,
                FileSizeBytes = bf.OriginalSize,
                StoredSizeBytes = bf.StoredSize,
                IsReadable = bf.IsReadable,
                CreatedAt = bf.CreatedAt,
                CompletedAt = bf.CompletedAt,
                //CreatedBy = bf.CreatedBy,
                //Version = bf.Version,
                BookFileStatusId = bf.StatusId,
                BookFileStatusName = bf.BookFileStatus?.Name
            }).ToList();
        }
    }

    public class GetBookFileHandler : IRequestHandler<GetBookFileQuery, Stream?>
    {
        private readonly IBookFileRepository _bookFileRepository;
        private readonly IStorageService _bookStorage;

        public GetBookFileHandler(IBookFileRepository bookFileRepository, IStorageService bookStorage)
        {
            _bookFileRepository = bookFileRepository;
            _bookStorage = bookStorage;
        }

        public async Task<Stream?> Handle(GetBookFileQuery request, CancellationToken cancellationToken)
        {
            var bookFile = await _bookFileRepository.GetByIdAsync(request.BookFileId, cancellationToken);
            if (bookFile == null || string.IsNullOrEmpty(bookFile.StorageUrl)) return null;
            string extension = ".fb2.zip";
            if (bookFile.FormatId == 2)
                extension = ".epub";
            return await _bookStorage.ReadBookSourceAsync(bookFile.Id.ToString(), extension, cancellationToken);
        }
    }


    public class UploadBookFileHandler : IRequestHandler<UploadBookFileCommand, long>
    {
        private readonly IBookFileRepository _bookFileRepository;
        private readonly IStorageService _bookStorage;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IBookConversionService _bookConversionService;
        private readonly IFb2Converter _converter;


        public UploadBookFileHandler(
            IBookFileRepository bookFileRepository,
            IStorageService bookStorage,
            IUnitOfWork unitOfWork,
            IFb2Converter converter

            //IBookConversionService bookConversionJob
            )
        {
            _bookFileRepository = bookFileRepository;
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
                StatusId = BookFileStatuses.PENDING
            };

            await _bookFileRepository.AddAsync(bookFile, cancellationToken);
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
                    compressedSize = buffer.Length; // EPUB и так архив
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

                _bookFileRepository.Update(bookFile);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                var bookFileId = bookFile.Id;

                if (request.IsReadable)
                //await _bookConversionService.ProcessAsync(bookFile.Id);
                {
                    buffer.Position = 0;


                    var result = await _converter.ConvertAsync(
                        buffer,
                        bookId: bookFile.Id,
                        options: new ConversionOptions { TargetPartSize = 80 }
                      );

                    await _bookFileRepository.SaveConversionResultAsync(bookFileId, result);

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
                    await _unitOfWork.SaveChangesAsync(CancellationToken.None); //чтобы не отменилось при отмене основного токена (а дефолт что это)
                }
                catch (Exception ex1)
                {
                    message = $"Ошибка при изменении данных о файле после неудачной загрузки";
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
            catch (Exception)
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
            catch (Exception)
            {
                throw new ChronolibrisException("Файл не является валидным XML — невалидный FB2", ErrorType.Validation);
            }
            finally
            {
                stream.Position = 0;
            }
        }
    }
    public class DeleteBookFileHandler : IRequestHandler<DeleteBookFileCommand, Unit>
    {
        private readonly IBookFileRepository _bookFileRepository;
        private readonly IStorageService _bookStorage;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBookFileHandler(
            IBookFileRepository bookFileRepository,
            IStorageService bookStorage,
            IUnitOfWork unitOfWork)
        {
            _bookFileRepository = bookFileRepository;
            _bookStorage = bookStorage;
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(DeleteBookFileCommand request, CancellationToken cancellationToken)
        {
            var bookFile = await _bookFileRepository.GetByIdAsync(request.BookFileId, cancellationToken);
            if (bookFile == null) 
                return Unit.Value;

            _bookFileRepository.Delete(bookFile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _bookStorage.DeleteBookDataAsync(bookFile.Id.ToString(), cancellationToken);

            return Unit.Value;
        }
    }

}