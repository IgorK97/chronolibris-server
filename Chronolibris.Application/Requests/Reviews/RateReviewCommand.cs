using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Reviews
{
    public record RateReviewCommand(long ReviewId, long UserId, short Score) : IRequest<ReviewDetails?>;
}
