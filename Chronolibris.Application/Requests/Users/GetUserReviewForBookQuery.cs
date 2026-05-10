using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Users
{
    public record GetUserReviewForBookQuery(long BookId, long UserId) : IRequest<MyReviewDetails?>;

}
