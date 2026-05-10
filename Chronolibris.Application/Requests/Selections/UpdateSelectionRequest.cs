using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record UpdateSelectionRequest(
            long SelectionId,
            [MaxLength(500)]
            string? Name,
            [MaxLength(2000)]
            string? Description,
            bool? IsActive
        ) : IRequest<bool>;
}
