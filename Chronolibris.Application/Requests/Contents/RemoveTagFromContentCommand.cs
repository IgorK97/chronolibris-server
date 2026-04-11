using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.Contents
{
    public record RemoveTagFromContentCommand(
        long ContentId,
        long TagId
    ) : IRequest<bool>;
}
