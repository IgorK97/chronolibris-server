using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests
{
    public record CreateSelectionRequest(
       string Name,
       string Description,
       int SelectionTypeId,long UserId
   ) : IRequest<long>;
}
