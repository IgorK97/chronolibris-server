using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Users
{
    public record UpdateUserProfileCommand(string FirstName, string LastName, string? Email, long UserId, string? PhoneNumber, string UserName) : IRequest<UserProfileResponse>;
}
