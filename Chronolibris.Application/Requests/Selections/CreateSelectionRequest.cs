using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record CreateSelectionInputModel(
        [MaxLength(500)]
        string Name,
        [MaxLength(2000)]
        string Description);
    public record CreateSelectionRequest(
       string Name,
       string Description,
       long UserId
   ) : IRequest<long>;
}
