using Chronolibris.Application.Requests;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Interfaces.Services;
using Chronolibris.Domain.Models;
using Chronolibris.Domain.Entities;
using MediatR;

namespace Chronolibris.Application.Commands
{
    public record CreateBookCommand(
        string Title,
        string Description,
        int CountryId,
        int LanguageId,
        int? Year,
        string? ISBN,
        string? Bbk,
        string? Udk,
        string? Source,
        string CoverBase64,         
        string CoverContentType,
        string CoverFileName,
        bool IsAvailable,
        bool IsReviewable,
        int? PublisherId,
        int? SeriesId,
        List<PersonRoleFilter>? PersonFilters,
        List<int>? ThemeIds
    ) : IRequest<long>;

    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, long>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IStorageService _storageService;

        public CreateBookCommandHandler(
            IBookRepository bookRepository,
            IStorageService storageService)
        {
            _bookRepository = bookRepository;
            _storageService = storageService;
        }

        public async Task<long> Handle(CreateBookCommand cmd, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cmd.Title))
                throw new ArgumentException("Название книги не может быть пустым.");

            if (string.IsNullOrWhiteSpace(cmd.CoverBase64))
                throw new ArgumentException("Файл обложки обязателен при создании книги.");

            // 1. Сохраняем запись в БД без coverPath
            var book = new Book
            {
                Id=0,
                Title = cmd.Title.Trim(),
                Description = cmd.Description.Trim(),
                CountryId = cmd.CountryId,
                LanguageId = cmd.LanguageId,
                Year = cmd.Year,
                ISBN = cmd.ISBN?.Trim(),
                Bbk = cmd.Bbk?.Trim(),
                Udk = cmd.Udk?.Trim(),
                Source = cmd.Source?.Trim(),
                CoverPath = "",
                IsAvailable = cmd.IsAvailable,
                IsReviewable = cmd.IsReviewable,
                PublisherId = cmd.PublisherId,
                //SeriesId = cmd.SeriesId,
                CreatedAt = DateTime.UtcNow,
            };

            var bookId = await _bookRepository.CreateAsync(book, cmd.PersonFilters, ct);

            // 2. Декодируем Base64 и загружаем обложку в MinIO: covers/{bookId}/cover.{ext}
            var imageBytes = DecodeCover(cmd.CoverBase64);
            var extension = Path.GetExtension(cmd.CoverFileName).ToLowerInvariant(); // ".jpg"
            var fileName = $"cover{extension}";
            var coverPath = $"covers/{bookId}/{fileName}";

            await _storageService.SaveCoverAsync(
                bookId.ToString(), fileName, imageBytes, cmd.CoverContentType, ct);

            book.CoverPath = coverPath;
            _bookRepository.Update(book);
            await _bookRepository.SaveChangesAsync();

            return bookId;
        }

        /// <summary>
        /// Принимает Base64 как с префиксом data URI ("data:image/jpeg;base64,..."),
        /// так и без него (чистый Base64).
        /// </summary>
        private static byte[] DecodeCover(string base64)
        {
            var data = base64.Contains(',')
                ? base64[(base64.IndexOf(',') + 1)..]
                : base64;

            return Convert.FromBase64String(data);
        }
    }
}