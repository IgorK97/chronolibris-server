using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Requests.Books
{
    //public record GetBooksQuery(BookFilterRequest Filter) : IRequest<BookListResponse>;

    //public record GetBookByIdQuery(long Id) : IRequest<BookDto?>;

    public record GetBookContentsQuery(long BookId) : IRequest<List<ContentDto>>;
    //public record DeleteBookCommand(long Id) : IRequest<Unit>;

    //public record LinkContentToBookCommand(long BookId, long ContentId) : IRequest<Unit>;

    //public record UnlinkContentFromBookCommand(long BookId, long ContentId) : IRequest<Unit>;


}