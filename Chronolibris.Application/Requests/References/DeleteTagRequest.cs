using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.References
{
    public record DeleteTagRequest(long TagId) : IRequest<bool>;
}
