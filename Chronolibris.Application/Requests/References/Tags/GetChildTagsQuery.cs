using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.References.Tags
{
    public record GetChildTagsQuery(long parentId,long? lastId,int pageSize) : IRequest<PagedResult<TagDetails>>;
}
