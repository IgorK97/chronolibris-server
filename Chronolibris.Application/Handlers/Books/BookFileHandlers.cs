using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                FileSizeBytes = bf.FileSizeBytes,
                IsReadable = bf.IsReadable,
                CreatedAt = bf.CreatedAt,
                CompletedAt = bf.CompletedAt,
                CreatedBy = bf.CreatedBy,
                //Version = bf.Version,
                BookFileStatusId = bf.BookFileStatusId,
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

            return await _bookStorage.ReadBookSourceAsync(bookFile.Id.ToString(), ".fb2", cancellationToken);
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
                !request.IsReadable && request.FormatId == 1)
                throw new ChronolibrisException("Неверно указан формат и режим использования книги", ErrorType.Validation);

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
                FileSizeBytes = request.FileSizeBytes,
                IsReadable = request.IsReadable,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy,
                BookFileStatusId = BookFileStatuses.PENDING
            };

            await _bookFileRepository.AddAsync(bookFile, cancellationToken);
            //await _unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
                using var buffer = new MemoryStream();
                await request.FileStream.CopyToAsync(buffer, cancellationToken);
                buffer.Position = 0;

                var storageUrl = await _bookStorage.SaveBookSourceAsync(
                    bookFile.Id.ToString(),
                    extension,
                    buffer,
                    cancellationToken);

                bookFile.StorageUrl = storageUrl;
                bookFile.BookFileStatusId = request.IsReadable ? BookFileStatuses.UPLOADED : BookFileStatuses.COMPLETED;
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
            catch
            {
                string message = "";
                try
                {
                    _bookFileRepository.Delete(bookFile);
                    await _unitOfWork.SaveChangesAsync(CancellationToken.None); //чтобы удаление не отменилось при отмене основного токена
                }
                catch (Exception ex)
                {
                    message = $"Ошибка при очистке данных о файле после неудачной загрузки";
                }
                throw new ChronolibrisException("Ошибка при создании файла: проблема с хранилищем файлов. " + message, ErrorType.ServerException);
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