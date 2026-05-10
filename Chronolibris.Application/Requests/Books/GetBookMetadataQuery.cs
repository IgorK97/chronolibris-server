using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Books
{
    public record GetBookMetadataQuery(long BookId, long UserId, bool Mode) : IRequest<BookDetails>;
}
