// File: Chronolibris.Application.Requests.BookRequests.cs
using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Requests
{
    // Queries
    public class GetBooksQuery : IRequest<BookListResponse>
    {
        public BookFilterRequest Filter { get; set; } = new();

        public GetBooksQuery(BookFilterRequest filter)
        {
            Filter = filter;
        }
    }

    public class GetBookByIdQuery : IRequest<BookDto?>
    {
        public long Id { get; set; }
        public GetBookByIdQuery(long id) => Id = id;
    }

    public class GetBookContentsQuery : IRequest<List<ContentDto>>
    {
        public long BookId { get; set; }
        public GetBookContentsQuery(long bookId) => BookId = bookId;
    }

    // Commands
    public class CreateBookCommand : IRequest<long>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long CountryId { get; set; }
        public long LanguageId { get; set; }
        public int? Year { get; set; }
        public string? ISBN { get; set; }
        public string? FilePath { get; set; }
        public string? CoverPath { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsReviewable { get; set; } = false;
        public long? PublisherId { get; set; }
        public long? SeriesId { get; set; }
        public List<long> PersonIds { get; set; } = new();
        public List<long> ThemeIds { get; set; } = new();

        public CreateBookCommand(string title, string description, long countryId,
            long languageId, int? year, string? isbn, string? filePath, string? coverPath,
            bool isAvailable, bool isReviewable, long? publisherId, long? seriesId,
            List<long> personIds, List<long> themeIds)
        {
            Title = title;
            Description = description;
            CountryId = countryId;
            LanguageId = languageId;
            Year = year;
            ISBN = isbn;
            FilePath = filePath;
            CoverPath = coverPath;
            IsAvailable = isAvailable;
            IsReviewable = isReviewable;
            PublisherId = publisherId;
            SeriesId = seriesId;
            PersonIds = personIds;
            ThemeIds = themeIds;
        }
    }

    //public class UpdateBookCommand : IRequest<Unit>
    //{
    //    public long Id { get; set; }
    //    public string Title { get; set; } = string.Empty;
    //    public string Description { get; set; } = string.Empty;
    //    public long CountryId { get; set; }
    //    public long LanguageId { get; set; }
    //    public int? Year { get; set; }
    //    public string? ISBN { get; set; }
    //    public string? FilePath { get; set; }
    //    public string? CoverPath { get; set; }
    //    public bool IsAvailable { get; set; } = true;
    //    public bool IsReviewable { get; set; } = false;
    //    public long? PublisherId { get; set; }
    //    public long? SeriesId { get; set; }
    //    public List<long> PersonIds { get; set; } = new();
    //    public List<long> ThemeIds { get; set; } = new();

    //    public UpdateBookCommand(long id, string title, string description, long countryId,
    //        long languageId, int? year, string? isbn, string? filePath, string? coverPath,
    //        bool isAvailable, bool isReviewable, long? publisherId, long? seriesId,
    //        List<long> personIds, List<long> themeIds)
    //    {
    //        Id = id;
    //        Title = title;
    //        Description = description;
    //        CountryId = countryId;
    //        LanguageId = languageId;
    //        Year = year;
    //        ISBN = isbn;
    //        FilePath = filePath;
    //        CoverPath = coverPath;
    //        IsAvailable = isAvailable;
    //        IsReviewable = isReviewable;
    //        PublisherId = publisherId;
    //        SeriesId = seriesId;
    //        PersonIds = personIds;
    //        ThemeIds = themeIds;
    //    }
    //}

    public class DeleteBookCommand : IRequest<Unit>
    {
        public long Id { get; set; }
        public DeleteBookCommand(long id) => Id = id;
    }

    public class LinkContentToBookCommand : IRequest<Unit>
    {
        public long BookId { get; set; }
        public long ContentId { get; set; }
        public int Order { get; set; }

        public LinkContentToBookCommand(long bookId, long contentId, int order)
        {
            BookId = bookId;
            ContentId = contentId;
            Order = order;
        }
    }

    public class UnlinkContentFromBookCommand : IRequest<Unit>
    {
        public long BookId { get; set; }
        public long ContentId { get; set; }

        public UnlinkContentFromBookCommand(long bookId, long contentId)
        {
            BookId = bookId;
            ContentId = contentId;
        }
    }


}