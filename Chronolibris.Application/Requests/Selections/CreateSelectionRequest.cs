using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record CreateSelectionInputModel(
        string Name, string Description);
    public record CreateSelectionRequest(
       string Name,
       string Description,
       long UserId
   ) : IRequest<long>;
}
