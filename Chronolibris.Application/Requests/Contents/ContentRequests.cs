using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Requests.Contents
{
    public class GetContentsQuery : IRequest<ContentListResponse>
    {
        public ContentFilterRequest Filter { get; set; } = new();

        public GetContentsQuery(ContentFilterRequest filter)
        {
            Filter = filter;
        }
    }

    public class GetContentByIdQuery : IRequest<ContentDto?>
    {
        public long Id { get; set; }
        public GetContentByIdQuery(long id) => Id = id;
    }

    public class GetContentBooksQuery : IRequest<List<BookDto>>
    {
        public long ContentId { get; set; }
        public GetContentBooksQuery(long contentId) => ContentId = contentId;
    }
    public class CreateContentCommand : IRequest<long>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long CountryId { get; set; }
        public long ContentTypeId { get; set; }
        public long LanguageId { get; set; }
        public int? Year { get; set; }
        public long? ParentContentId { get; set; }
        public int? Position { get; set; }
        public List<long> PersonIds { get; set; } = new();
        public List<long> ThemeIds { get; set; } = new();

        public CreateContentCommand(string title, string description, long countryId,
            long contentTypeId, long languageId, int? year, long? parentContentId,
            int? position, List<long> personIds, List<long> themeIds)
        {
            Title = title;
            Description = description;
            CountryId = countryId;
            ContentTypeId = contentTypeId;
            LanguageId = languageId;
            Year = year;
            ParentContentId = parentContentId;
            Position = position;
            PersonIds = personIds;
            ThemeIds = themeIds;
        }
    }

    public class DeleteContentCommand : IRequest<Unit>
    {
        public long Id { get; set; }
        public DeleteContentCommand(long id) => Id = id;
    }

    public class LinkBookToContentCommand : IRequest<Unit>
    {
        public long ContentId { get; set; }
        public long BookId { get; set; }
        public int Order { get; set; }

        public LinkBookToContentCommand(long contentId, long bookId, int order)
        {
            ContentId = contentId;
            BookId = bookId;
            Order = order;
        }
    }

    public class UnlinkBookFromContentCommand : IRequest<Unit>
    {
        public long ContentId { get; set; }
        public long BookId { get; set; }

        public UnlinkBookFromContentCommand(long contentId, long bookId)
        {
            ContentId = contentId;
            BookId = bookId;
        }
    }
}