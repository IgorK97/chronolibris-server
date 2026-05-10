using MediatR;

namespace Chronolibris.Application.Requests.Books
{
    public record GetTocQuery(long BookFileId) : IRequest<string?>;
}
