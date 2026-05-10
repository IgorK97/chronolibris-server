using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record SeekBookInSelectionsQuery(long BookId) : IRequest<List<long>>;
}
