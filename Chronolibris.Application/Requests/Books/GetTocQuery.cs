using MediatR;

namespace Chronolibris.Application.Requests.Books
{
    public record GetTocQuery(long BookFileId, long UserId, string Role) : IRequest<string?>;
}
