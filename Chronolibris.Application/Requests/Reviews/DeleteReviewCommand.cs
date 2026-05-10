using MediatR;

namespace Chronolibris.Application.Requests.Reviews
{
    public record DeleteReviewCommand(long ReviewId, long UserId) : IRequest<Unit>;
}
