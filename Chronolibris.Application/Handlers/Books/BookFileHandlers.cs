using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Fb2Converter.Interfaces;
using Chronolibris.Application.Jobs;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Books;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
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

    public class GetBookFileDtoHandler : IRequestHandler<GetBookFileDtoQuery, BookFileDto?>
    {
        private readonly IBookFileRepository _bookFileRepository;

        public GetBookFileDtoHandler(IBookFileRepository bookFileRepository)
        {
            _bookFileRepository = bookFileRepository;
        }

        public async Task<BookFileDto?> Handle(GetBookFileDtoQuery request, CancellationToken cancellationToken)
        {
            var bookFile = await _bookFileRepository.GetByIdAsync(request.BookFileId, cancellationToken);
            if (bookFile == null) return null;

            return new BookFileDto
            {
                Id = bookFile.Id,
                BookId = bookFile.BookId,
                FormatId = bookFile.FormatId,
                FormatName = bookFile.Format?.Name,
                StorageUrl = bookFile.StorageUrl,
                FileSizeBytes = bookFile.FileSizeBytes,
                IsReadable = bookFile.IsReadable,
                CreatedAt = bookFile.CreatedAt,
                CompletedAt = bookFile.CompletedAt,
                CreatedBy = bookFile.CreatedBy,
                //Version = bookFile.Version,
                BookFileStatusId = bookFile.BookFileStatusId,
                BookFileStatusName = bookFile.BookFileStatus?.Name
            };
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
            if (bookFile == null) return null;

            return await _bookStorage.ReadByStorageUrlAsync(bookFile.StorageUrl, cancellationToken);
        }
    }


    public class UploadBookFileHandler : IRequestHandler<UploadBookFileCommand, long>
    {
        private readonly IBookFileRepository _bookFileRepository;
        private readonly IStorageService _bookStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookConversionJob _bookConversionJob;
        //private readonly IBackgroundJobClient _backgroundJobs;

        public UploadBookFileHandler(
            IBookFileRepository bookFileRepository,
            IStorageService bookStorage,
            IUnitOfWork unitOfWork,
            IBookConversionJob bookConversionJob)
        {
            _bookFileRepository = bookFileRepository;
            _bookStorage = bookStorage;
            _unitOfWork = unitOfWork;
            _bookConversionJob = bookConversionJob;
        }

        public async Task<long> Handle(UploadBookFileCommand request, CancellationToken cancellationToken)
        {
            const long MAX_FILE_SIZE = 100 * 1024 * 1024;
            if (request.FileSizeBytes > MAX_FILE_SIZE)
                throw new ArgumentException("Размер файла не должен превышать 100 MB");

            var existingFile = await _bookFileRepository.GetByBookIdAndFormatIdAsync(
                request.BookId, request.FormatId, cancellationToken);
            if (existingFile != null)
                throw new ArgumentException($"Файл формата {request.FormatId} уже существует для этой книги");

            var bookFile = new BookFile
            {
                Id = 0,
                BookId = request.BookId,
                FormatId = request.FormatId,
                StorageUrl = string.Empty,
                FileSizeBytes = request.FileSizeBytes,
                IsReadable = request.IsReadable,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy,
                //Version = 0,
                BookFileStatusId = BookFileStatuses.PENDING
            };

            await _bookFileRepository.AddAsync(bookFile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                // расширение из имени файла — единственный источник правды
                var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

                // метод сам строит ключ и возвращает storageUrl
                var storageUrl = await _bookStorage.SaveBookSourceAsync(
                    bookFile.Id.ToString(),
                    extension,
                    request.FileStream,
                    cancellationToken);

                bookFile.StorageUrl = storageUrl;
                bookFile.BookFileStatusId = BookFileStatuses.UPLOADED;
                bookFile.CompletedAt = DateTime.UtcNow;

                _bookFileRepository.Update(bookFile);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (request.IsReadable)
                    await _bookConversionJob.ProcessAsync(bookFile.Id);
                //{
                //    if (request.IsReadable)
                //        _bookConversionJob.Enqueue<IBookConversionJob>(job => job.ProcessAsync(bookFile.Id));
                //}

                return bookFile.Id;
            }
            catch
            {
                _bookFileRepository.Delete(bookFile);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                throw;
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
            if (bookFile == null) throw new KeyNotFoundException($"BookFile with ID {request.BookFileId} not found");

            // Удаляем файл из MinIO
            try
            {
                await _bookStorage.DeleteFileAsync(bookFile.StorageUrl, cancellationToken);
            }
            catch
            {
                // Игнорируем ошибки удаления из хранилища
            }

            // Удаляем запись из БД
            _bookFileRepository.Delete(bookFile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
    public class UpdateBookFileHandler : IRequestHandler<UpdateBookFileCommand, long>
    {
        private readonly IBookFileRepository _bookFileRepository;
        private readonly IStorageService _bookStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookConversionJob _bookConversionJob;

        public UpdateBookFileHandler(
            IBookFileRepository bookFileRepository,
            IStorageService bookStorage,
            IUnitOfWork unitOfWork,
            IBookConversionJob bookConversionJob)
        {
            _bookFileRepository = bookFileRepository;
            _bookStorage = bookStorage;
            _unitOfWork = unitOfWork;
            _bookConversionJob = bookConversionJob;
        }
        public async Task<long> Handle(UpdateBookFileCommand request, CancellationToken cancellationToken)
        {
            const long MAX_FILE_SIZE = 100 * 1024 * 1024;
            if (request.FileSizeBytes > MAX_FILE_SIZE)
                throw new ArgumentException("Размер файла не должен превышать 100 MB");

            var existingFile = await _bookFileRepository.GetByBookIdAndFormatIdAsync(
                request.BookId, request.FormatId, cancellationToken);
            if (existingFile == null)
                throw new KeyNotFoundException($"BookFile not found for book {request.BookId} and format {request.FormatId}");

            // Удаляем старый файл — DeleteFileAsync идемпотентен, catch не нужен
            await _bookStorage.DeleteFileAsync(existingFile.StorageUrl, cancellationToken);

            var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
            var storageUrl = await _bookStorage.SaveBookSourceAsync(
                existingFile.BookId.ToString(),
                extension,
                request.FileStream,
                cancellationToken);

            existingFile.StorageUrl = storageUrl;
            existingFile.FileSizeBytes = request.FileSizeBytes;
            existingFile.IsReadable = request.IsReadable;
            //existingFile.Version++;
            existingFile.BookFileStatusId = BookFileStatuses.UPLOADED;
            existingFile.CompletedAt = DateTime.UtcNow;

            _bookFileRepository.Update(existingFile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.IsReadable)
                await _bookConversionJob.ProcessAsync(existingFile.Id);

            return existingFile.Id;
        }
    }
    public class ProcessBookFileHandler : IRequestHandler<ProcessBookFileCommand, Unit>
    {
        private readonly IBookFileRepository _bookFileRepository;
        private readonly IFb2Converter _fb2Converter;
        private readonly IUnitOfWork _unitOfWork;

        public ProcessBookFileHandler(
            IBookFileRepository bookFileRepository,
            IFb2Converter fb2Converter,
            IUnitOfWork unitOfWork)
        {
            _bookFileRepository = bookFileRepository;
            _fb2Converter = fb2Converter;
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(ProcessBookFileCommand request, CancellationToken cancellationToken)
        {
            var bookFile = await _bookFileRepository.GetByIdAsync(request.BookFileId, cancellationToken);
            if (bookFile == null) throw new KeyNotFoundException($"BookFile with ID {request.BookFileId} not found");

            // Обновляем статус на PROCESSING (3)
            bookFile.BookFileStatusId = BookFileStatuses.PROCESSING;
            _bookFileRepository.Update(bookFile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                // Конвертация FB2
                // var stream = await _bookStorage.ReadStreamAsync(bookFile.StorageUrl, cancellationToken);
                // await _fb2Converter.ConvertAsync(stream, bookFile.BookId, null, cancellationToken);

                // Обновляем статус на COMPLETED (4)
                bookFile.BookFileStatusId = BookFileStatuses.COMPLETED;
                bookFile.CompletedAt = DateTime.UtcNow;
                _bookFileRepository.Update(bookFile);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                // Обновляем статус на FAILED (5)
                bookFile.BookFileStatusId = BookFileStatuses.FAILED;
                _bookFileRepository.Update(bookFile);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                throw;
            }

            return Unit.Value;
        }
    }
}
