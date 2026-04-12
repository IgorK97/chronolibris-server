using Chronolibris.Application.Requests;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Services;
using Chronolibris.Domain.Models;
using Chronolibris.Domain.Entities;
using MediatR;
using Chronolibris.Domain.Interfaces.Repository;

namespace Chronolibris.Application.Handlers.Books
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

            var imageBytes = DecodeCover(cmd.CoverBase64);
            var extension = Path.GetExtension(cmd.CoverFileName).ToLowerInvariant();
            var fileName = $"cover{extension}";
            var coverPath = $"covers/{bookId}/{fileName}";

            await _storageService.SaveCoverAsync(
                bookId.ToString(), fileName, imageBytes, cmd.CoverContentType, ct);

            book.CoverPath = coverPath;
            _bookRepository.Update(book);
            await _bookRepository.SaveChangesAsync();

            return bookId;
        }
        private static byte[] DecodeCover(string base64)
        {
            var data = base64.Contains(',')
                ? base64[(base64.IndexOf(',') + 1)..]
                : base64;

            return Convert.FromBase64String(data);
        }
    }
}