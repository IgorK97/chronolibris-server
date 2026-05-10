using MediatR;

namespace Chronolibris.Application.Requests.Shelves
{
    public record SeekBookInShelvesQuery(long UserId, long BookId) : IRequest<long[]>;
}
