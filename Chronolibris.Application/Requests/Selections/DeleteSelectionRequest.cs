using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record DeleteSelectionRequest(long SelectionId) : IRequest<bool>;
}
