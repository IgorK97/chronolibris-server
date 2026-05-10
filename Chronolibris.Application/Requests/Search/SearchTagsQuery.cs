using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Search
{
    public record GetTagsQuery(
     string SearchTerm,
     long? TagTypeId = null,
     int Limit = 5
 ) : IRequest<List<TagDetails>>;
}
