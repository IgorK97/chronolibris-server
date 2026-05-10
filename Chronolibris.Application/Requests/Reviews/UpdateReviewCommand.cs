using MediatR;

namespace Chronolibris.Application.Requests.Reviews
{
    public record UpdateReviewCommand(long ReviewId, long UserId, string? ReviewText, short Score) : IRequest<Unit>;
}
