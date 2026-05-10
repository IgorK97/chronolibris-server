using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Interfaces.Services;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Books
{
    public record UpdateBookCommand(
        long Id,
        string Title,
        string Description,
        long? CountryId,
        long? LanguageId,
        int? Year, bool YearProvided,
        string? ISBN, bool IsbnProvided,
        string? Bbk, bool BbkProvided,
        string? Udk, bool UdkProvided,
        string? Source, bool SourceProvided,
        string? CoverBase64,
        bool IsAvailable,
        bool IsReviewable,
        long? PublisherId, bool PublisherIdProvided,
        //int? SeriesId, bool SeriesIdProvided,
        List<PersonRoleFilter>? PersonFilters,
        List<int>? ThemeIds,
        bool DeleteCoverCommand,
        bool HasHistoricalVersions
    ) : IRequest;

    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public UpdateBookCommandHandler(
            IUnitOfWork bookRepository,
            IStorageService storageService)
        {
            _unitOfWork = bookRepository;
            _storageService = storageService;
        }

        public async Task Handle(UpdateBookCommand cmd, CancellationToken ct)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(cmd.Id, ct)
                ?? throw new ChronolibrisException("Книга не найдена", ErrorType.NotFound);
            DateTime date = DateTime.UtcNow;

            UpdateBookFields(book, cmd);
            book.UpdatedAt = date;

            if (!string.IsNullOrWhiteSpace(cmd.CoverBase64))
            {

                //var newExt = Path.GetExtension(cmd.CoverFileName ?? ".jpg").ToLowerInvariant();
                //var fileName = $"cover{newExt}";
                //var newCoverPath = $"covers/{cmd.Id}/{fileName}";
                var imageBytes = GetBytesFromBase64(cmd.CoverBase64);

                var (extension, contentType) = GetFileInfo(imageBytes);

                try
                {
                    //using var imageBytes = DecodeCover(cmd.CoverBase64);
                    var fileName = $"cover{extension}";
                    var coverPath = $"covers/{cmd.Id}/{fileName}";
                    using (var imageStream = new MemoryStream(imageBytes))
                    {
                        await _storageService.SaveCoverAsync(
                        cmd.Id.ToString(), fileName, imageStream, contentType ?? "image/jpeg", ct);
                    }
                        
                    var oldPath = book.CoverPath;
                    book.CoverPath = coverPath;

                    if (oldPath != null && !oldPath.EndsWith(extension))
                    {
                        await _storageService.DeleteFileAsync("images", oldPath, ct);
                    }

                }
                catch (Exception ex)
                {
                }
            }
            else if(cmd.DeleteCoverCommand)
            {
                var oldPath = book.CoverPath;
                book.CoverPath = "";

                if (oldPath != null)
                {
                    await _storageService.DeleteFileAsync("images", oldPath, ct);
                }
            }
            if (cmd.PersonFilters != null)
            {
                _unitOfWork.Books.SyncParticipations(book, cmd.PersonFilters);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }

        private static byte[] GetBytesFromBase64(string base64)
        {
            try
            {
                var data = base64.Contains(',')
                ? base64[(base64.IndexOf(',') + 1)..]
                : base64;

                return Convert.FromBase64String(data);
            }
            catch (Exception ex)
            {
                throw new ChronolibrisException("Невалидная строка base64", ErrorType.Validation);
            }
        }

        private static (string Extension, string ContentType) GetFileInfo(byte[] bytes)
        {
            if (bytes.Length < 4)
                throw new ChronolibrisException("Некорректное изображение", ErrorType.Validation);

            //PNG, \x89PNG
            if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                return (".png", "image/png");

            //JPEG, SOI-префикс
            if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                return (".jpg", "image/jpeg");

            //WebP, в начале RIFF, потом 4 байта длины, потом WEBP
            if (bytes.Length >= 12 &&
                bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
                bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
            {
                return (".webp", "image/webp");
            }

            throw new ChronolibrisException("Некорректное изображение", ErrorType.Validation);
        }

        private void UpdateBookFields(Book book, UpdateBookCommand cmd)
        {
            book.Title = cmd.Title.Trim();
            book.Description = cmd.Description.Trim();
            book.IsAvailable = cmd.IsAvailable;
            book.IsReviewable = cmd.IsReviewable;
            book.HasHistoricalVersions = cmd.HasHistoricalVersions;


            if (cmd.CountryId != null) book.CountryId = cmd.CountryId.Value;
            if (cmd.LanguageId != null) book.LanguageId = cmd.LanguageId.Value;

            if (cmd.YearProvided) book.Year = cmd.Year;
            if (cmd.IsbnProvided) book.ISBN = cmd.ISBN?.Trim();
            if (cmd.BbkProvided) book.Bbk = cmd.Bbk?.Trim();
            if (cmd.UdkProvided) book.Udk = cmd.Udk?.Trim();
            if (cmd.SourceProvided) book.Source = cmd.Source?.Trim();
            if (cmd.PublisherIdProvided) book.PublisherId = cmd.PublisherId;

            book.UpdatedAt = DateTime.UtcNow;
        }
    }
}