using Chronolibris.Application.Requests;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Interfaces.Services;
using Chronolibris.Domain.Models;
using MediatR;

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
        string? CoverBase64,         
        string? CoverContentType,
        string? CoverFileName,
        bool IsAvailable,
        bool IsReviewable,
        int? PublisherId,
        List<PersonRoleFilter>? PersonFilters
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

            if (!string.IsNullOrEmpty(cmd.CoverBase64) && !string.IsNullOrEmpty(cmd.CoverFileName) && !string.IsNullOrEmpty(cmd.CoverContentType))
            {
                try
                {
                    //var imageBytes = DecodeCover(cmd.CoverBase64);
                    var extension = Path.GetExtension(cmd.CoverFileName).ToLowerInvariant();
                    var fileName = $"cover{extension}";
                    var coverPath = $"covers/{bookId}/{fileName}";
                    using (var imageStream = DecodeCover(cmd.CoverBase64))
                    {
                        await _storageService.SaveCoverAsync(
                        bookId.ToString(), fileName, imageStream, cmd.CoverContentType, ct);
                    }

                    //    await _storageService.SavePublicBookImageAsync(
                    //bookId.ToString(), fileName, imageBytes, cmd.CoverContentType, ct);

                    book.CoverPath = coverPath;
                    _bookRepository.Update(book);
                    await _bookRepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    
                }
            }

            return bookId;
        }
        private static Stream DecodeCover(string base64)
        {

            var data = base64.Contains(',')
                        ? base64[(base64.IndexOf(',') + 1)..]
                        : base64;

            var bytes = Convert.FromBase64String(data);

            return new MemoryStream(bytes);
        }
    }
}