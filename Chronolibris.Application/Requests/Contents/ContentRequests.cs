using MediatR;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Requests.Contents
{
    public record GetContentsQuery(ContentFilterRequest Filter) : IRequest<PagedResult<ContentDto>>;

    public record GetContentByIdQuery(long Id) : IRequest<ContentDto?>;

    public record GetContentBooksQuery(long ContentId) : IRequest<List<BookDto>>;
    public record CreateContentCommand(string Title, string Description, long CountryId, long ContentTypeId,
        long LanguageId, int? YearFrom, int? YearTo, List<PersonRoleFilter> PersonFilters, List<long>ThemeIds) : IRequest<long>;
    

    public record DeleteContentCommand(long Id) : IRequest<Unit>;

    public record LinkBookToContentCommand(long ContentId, long BookId) : IRequest<Unit>;

    public record UnlinkBookFromContentCommand(long ContentId, long BookId) : IRequest<Unit>;
}