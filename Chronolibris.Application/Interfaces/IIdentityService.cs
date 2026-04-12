using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Users;

namespace Chronolibris.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<RegistrationResult> RegisterUserAsync(RegisterRequest request);
        Task<LoginResult> LoginUserByEmailAsync(string Email, string Password);
        Task<LoginResult> LoginUserByUserNameAsync(string userName, string password);
        Task<bool> IsUserNameUniqueAsync(string userName);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool>IsPhoneUniqueAsync(string phone);
        Task<bool> IsUserActiveAsync(long userId);
        Task<UserProfileResponse?> GetUserProfileAsync(long userId);
        Task<UserProfileResponse> UpdateUserProfileAsync(UpdateUserProfileCommand request);
        Task<bool> ChangePasswordAsync(ChangePasswordCommand request);
    }
}
