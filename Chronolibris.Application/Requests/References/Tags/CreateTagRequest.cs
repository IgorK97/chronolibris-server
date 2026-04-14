using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.References.Tags
{
    public record CreateTagRequest(
        [MaxLength(500)]
        string Name,
        long TagTypeId,
        long? ParentTagId,
        long? RelationTypeId
    ) : IRequest<long>;
}
