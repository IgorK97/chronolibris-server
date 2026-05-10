using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record RemoveBookFromSelectionRequest(
        long SelectionId,
        long BookId
    ) : IRequest<bool>;
}
