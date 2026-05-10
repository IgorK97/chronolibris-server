using Chronolibris.Application.Models;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Reviews
{
    public record GetReviewsQuery(long BookId, long? LastId, int Limit, long? UserId=null):IRequest<PagedResult<ReviewDetails>>;
}
