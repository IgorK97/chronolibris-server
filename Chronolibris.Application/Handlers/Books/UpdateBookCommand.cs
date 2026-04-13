using Chronolibris.Application.Requests;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;

//using Chronolibris.Domain.Interfaces.Repositories;
using Chronolibris.Domain.Interfaces.Services;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Books
{
    public record UpdateBookCommand(
        long Id,
        string Title,
        string Description,
        int? CountryId,
        int? LanguageId,
        int? Year, bool YearProvided,
        string? ISBN, bool IsbnProvided,
        string? Bbk, bool BbkProvided,
        string? Udk, bool UdkProvided,
        string? Source, bool SourceProvided,
        string? CoverBase64,        
        string? CoverContentType,
        string? CoverFileName,
        bool IsAvailable,
        bool IsReviewable,
        int? PublisherId, bool PublisherIdProvided,
        //int? SeriesId, bool SeriesIdProvided,
        List<PersonRoleFilter>? PersonFilters,
        List<int>? ThemeIds
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

            UpdateBookFields(book, cmd);

            if (!string.IsNullOrWhiteSpace(cmd.CoverBase64))
            {
                var imageBytes = DecodeCover(cmd.CoverBase64);
                var newExt = Path.GetExtension(cmd.CoverFileName ?? ".jpg").ToLowerInvariant();
                var fileName = $"cover{newExt}";
                var newCoverPath = $"covers/{cmd.Id}/{fileName}";

                try
                {
                    await _storageService.SavePublicBookImageAsync(
                    cmd.Id.ToString(), fileName, imageBytes, cmd.CoverContentType ?? "image/jpeg", ct);
                    var oldPath = book.CoverPath;
                    book.CoverPath = newCoverPath;

                    if (oldPath != null && !oldPath.EndsWith(newExt))
                    {
                        await _storageService.DeleteAsync(_storageService.PublicCoversBucket, oldPath, ct);
                    }

                }
                catch (Exception ex)
                {
                }
            }

            if (cmd.PersonFilters != null)
            {
                _unitOfWork.Books.SyncParticipations(book, cmd.PersonFilters);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }

        private void UpdateBookFields(Book book, UpdateBookCommand cmd)
        {
            book.Title = cmd.Title.Trim();
            book.Description = cmd.Description.Trim();
            book.IsAvailable = cmd.IsAvailable;
            book.IsReviewable = cmd.IsReviewable;


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

        //public async Task Handle(UpdateBookCommand cmd, CancellationToken ct)
        //{
        //    var book = await _bookRepository.GetByIdAsync(cmd.Id, ct)
        //        ?? throw new ChronolibrisException($"Такая книга не найдена", ErrorType.NotFound);


        //    book.Title = cmd.Title.Trim();
        //    book.Description = cmd.Description.Trim();
        //    book.IsAvailable = cmd.IsAvailable;
        //    book.IsReviewable = cmd.IsReviewable;


        //    if (cmd.CountryId != null) book.CountryId = cmd.CountryId.Value;
        //    if (cmd.LanguageId != null) book.LanguageId = cmd.LanguageId.Value;

        //    if (cmd.YearProvided) book.Year = cmd.Year;
        //    if (cmd.IsbnProvided) book.ISBN = cmd.ISBN?.Trim();
        //    if (cmd.BbkProvided) book.Bbk = cmd.Bbk?.Trim();
        //    if (cmd.UdkProvided) book.Udk = cmd.Udk?.Trim();
        //    if (cmd.SourceProvided) book.Source = cmd.Source?.Trim();
        //    if (cmd.PublisherIdProvided) book.PublisherId = cmd.PublisherId;

        //    book.UpdatedAt = DateTime.UtcNow;

        //    await _bookRepository.UpdateAsync(book, cmd.PersonFilters,ct);

        //    if (!string.IsNullOrWhiteSpace(cmd.CoverBase64))
        //    {
        //        var imageBytes = DecodeCover(cmd.CoverBase64);
        //        var newExt = Path.GetExtension(cmd.CoverFileName ?? "cover.jpg").ToLowerInvariant();
        //        var fileName = $"cover{newExt}";

        //        var existingExt = book.CoverPath is not null
        //            ? Path.GetExtension(book.CoverPath).ToLowerInvariant()
        //            : newExt;

        //        if (existingExt != newExt && !string.IsNullOrWhiteSpace(book.CoverPath))
        //        {
        //            await _storageService.DeleteAsync(_storageService.PublicCoversBucket, book.CoverPath, ct);
        //            var newCoverPath = $"covers/{cmd.Id}/{fileName}";
        //            book.CoverPath = newCoverPath;
        //            _bookRepository.Update(book);
        //            await _bookRepository.SaveChangesAsync();
        //        }

        //        // Загружаем новый файл (перезапись при совпадении имени MinIO обеспечивает сам)
        //        await _storageService.SavePublicBookImageAsync(
        //            cmd.Id.ToString(), fileName, imageBytes, cmd.CoverContentType ?? "image/jpeg", ct);
        //    }
        //}

        private static byte[] DecodeCover(string base64)
        {
            var data = base64.Contains(',')
                ? base64[(base64.IndexOf(',') + 1)..]
                : base64;

            return Convert.FromBase64String(data);
        }
    }
}