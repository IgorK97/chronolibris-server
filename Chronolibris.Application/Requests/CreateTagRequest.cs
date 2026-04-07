using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests
{
    public record CreateTagRequest(
        string Name,
        long TagTypeId,
        long? ParentTagId,
        long? RelationTypeId
    ) : IRequest<long>;
}
