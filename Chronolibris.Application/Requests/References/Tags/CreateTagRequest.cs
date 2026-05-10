using System.ComponentModel.DataAnnotations;
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
