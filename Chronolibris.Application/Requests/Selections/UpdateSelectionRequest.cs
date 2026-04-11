using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record UpdateSelectionRequest(
            long SelectionId,
            string? Name,
            string? Description,
            bool? IsActive
        ) : IRequest<bool>;
}
