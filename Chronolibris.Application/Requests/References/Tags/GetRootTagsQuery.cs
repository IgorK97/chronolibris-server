using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.References.Tags
{
    public record GetRootTagsQuery(long? tagTypeId, string? searchTerm, long? lastId, int pageSize) :IRequest<PagedResult<TagDetails>>;
}
