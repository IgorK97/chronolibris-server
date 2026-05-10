using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Users;

namespace Chronolibris.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<RegistrationResult> RegisterUserAsync(RegisterRequest request);
        Task<LoginResult> LoginUserByUserNameAsync(string userName, string password);
        Task<bool> IsUserActiveAsync(long userId);
        Task<UserProfileResponse?> GetUserProfileAsync(long userId);
        Task<UserProfileResponse> UpdateUserProfileAsync(UpdateUserProfileCommand request);
        Task ChangePasswordAsync(ChangePasswordCommand request);
    }
}
