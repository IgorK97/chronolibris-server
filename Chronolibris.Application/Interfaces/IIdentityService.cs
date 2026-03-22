using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;

namespace Chronolibris.Application.Interfaces
{
    /// <summary>
    /// Определяет контракт для сервисов, управляющих аутентификацией, 
    /// регистрацией и идентификацией пользователей в системе.
    /// </summary>
    public interface IIdentityService
    {

        /// <summary>
        /// Асинхронно регистрирует нового пользователя на основе предоставленных данных.
        /// </summary>
        /// <param name="request">Объект запроса, содержащий данные нового пользователя (например, Email, Password, Username).</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию. Результат задачи — 
        /// <see cref="RegistrationResult"/>, содержащий статус операции и возможные ошибки.
        /// </returns>
        Task<RegistrationResult> RegisterUserAsync(RegisterRequest request);

        /// <summary>
        /// Асинхронно аутентифицирует пользователя по его электронной почте и паролю.
        /// </summary>
        /// <param name="Email">Адрес электронной почты пользователя.</param>
        /// <param name="Password">Пароль пользователя.</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию. Результат задачи — 
        /// <see cref="LoginResult"/>, содержащий статус входа и, при успешной аутентификации, токен доступа или информацию о пользователе.
        /// </returns>
        Task<LoginResult> LoginUserByEmailAsync(string Email, string Password);
        Task<LoginResult> LoginUserByUserNameAsync(string userName, string password);
        Task<bool> IsUserNameUniqueAsync(string userName, string role);
        Task<bool> IsEmailUniqueAsync(string email, string role);
        Task<bool>IsPhoneUniqueAsync(string phone, string role);
        Task<string?> RefreshTokenAsync(string token);
        //Task<MeData?> GetMeDataAsync(Guid userId);

        Task<UserProfileResponse?> GetUserProfileAsync(long userId);
        /// <summary>
        /// Асинхронно обновляет имя и/или email пользователя.
        /// </summary>
        Task<UserProfileResponse> UpdateUserProfileAsync(UpdateUserProfileCommand request);

        /// <summary>
        /// Асинхронно меняет пароль пользователя.
        /// </summary>
        Task<bool> ChangePasswordAsync(ChangePasswordCommand request);
    }
}
