using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record AddBookToSelectionRequest(
        long SelectionId,
        long BookId
    ) : IRequest;
}
