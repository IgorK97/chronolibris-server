using MediatR;

namespace Chronolibris.Application.Requests.Reviews
{
    public record CreateReviewCommand(long BookId, long UserId, string? ReviewText, short Score) : IRequest<long>;
}
