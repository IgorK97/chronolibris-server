using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Users
{
    public record GetUserProfileQuery(long UserId): IRequest<UserProfileResponse?>;
}
