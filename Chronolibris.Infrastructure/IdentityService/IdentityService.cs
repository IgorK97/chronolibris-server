using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Chronolibris.Infrastructure.Identity
{
    /// <summary>
    /// Сервис, предоставляющий функциональность для управления пользователями (регистрация, вход) 
    /// и генерации токенов аутентификации на основе ASP.NET Core Identity.
    /// Реализует интерфейс <see cref="IIdentityService"/>.
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="IdentityService"/>.
        /// </summary>
        /// <param name="userManager">Менеджер пользователей ASP.NET Core Identity для управления сущностями <see cref="User"/>.</param>
        /// <param name="signInManager">Менеджер входа ASP.NET Core Identity для проверки учетных данных.</param>
        /// <param name="config">Конфигурация приложения, используемая для получения секретов JWT.</param>
        public IdentityService(UserManager<User> userManager, 
            SignInManager<User> signInManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        /// <summary>
        /// Асинхронно регистрирует нового пользователя в системе.
        /// </summary>
        /// <param name="request">Запрос на регистрацию, содержащий имя, фамилию, email и пароль.</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию. Результат задачи — 
        /// объект <see cref="RegistrationResult"/>, содержащий статус успеха, 
        /// ошибки (если есть) и JWT-токен при успешной регистрации.
        /// </returns>
        public async Task<RegistrationResult> RegisterUserAsync(RegisterRequest request)
        {
            DateTime dt = DateTime.UtcNow;
            var user = new User
            {
                LastName = request.LastName,
                IsDeleted = false,
                //LastEnteredAt = dt,
                FirstName = request.UserName,
                RegisteredAt = dt,
                Email = request.Email,
                UserName = request.UserName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return new RegistrationResult
                {
                    Success = false,
                    UserId=0,
                    Message = result.Errors.Select(e => e.Description).FirstOrDefault(),
                };
            };

            await _userManager.AddToRoleAsync(user, "Reader");

            var refreshToken = GenerateRefreshToken();
            //user.RefreshToken = refreshToken;
            //user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(365);
           
            await _userManager.UpdateAsync(user);
            return new RegistrationResult
            {
                Success = result.Succeeded,
                UserId = user.Id,
                Token = await GenerateJwtToken(user),
                RefreshToken = refreshToken,
                Message = result.Succeeded ? null : result.Errors.Select(e => e.Description).FirstOrDefault()
            };
        
        }

        /// <summary>
        /// Асинхронно выполняет вход пользователя по электронной почте и паролю.
        /// </summary>
        /// <param name="Email">Электронная почта пользователя.</param>
        /// <param name="Password">Пароль пользователя.</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию. Результат задачи — 
        /// объект <see cref="LoginResult"/>, содержащий статус успеха и JWT-токен при успешном входе. 
        /// Возвращает успешный результат с пустым токеном или с ошибками в случае неудачи (зависит от логики обработки ошибок).
        /// </returns>
        public async Task<LoginResult> LoginUserByEmailAsync(string Email, string Password)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null) 
                return new LoginResult { Success = false,
                    Token = string.Empty,
                    Message = "User not found"
                };
            var result = await _signInManager
                .CheckPasswordSignInAsync(user, Password, false);

            if (!result.Succeeded) 
                return new LoginResult 
                { 
                    Success = false,
                    Token = string.Empty,
                    Message = "Invalid credentials"
                };

            string jwt = await GenerateJwtToken(user);
            string refresh = GenerateRefreshToken();

            //user.RefreshToken = refresh;
            //user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new LoginResult
            {
                Success = true,
                Token = jwt,
                RefreshToken = refresh,
                Message = "Login successful"
            };
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        //public async Task<string?> RefreshTokenAsync(string refreshToken)
        //{
        //    var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        //    if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        //        return null;

        //    var newToken = await GenerateJwtToken(user);
        //    return newToken;
        //}



        /// <summary>
        /// Создает подписанный JSON Web Token (JWT) для указанного пользователя.
        /// </summary>
        /// <param name="user">Сущность <see cref="User"/>, для которого создается токен.</param>
        /// <returns>Сгенерированная строка JWT-токена.</returns>
        private async Task<string> GenerateJwtToken(User user)
        {

            //string res1 = _config["Jwt:Issuer"];
            //string res2 = _config["Jwt:Audience"];
            //string res3 = _config["Jwt:Key"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                //claims.Append(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(365),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserProfileResponse?> GetUserProfileAsync(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return null;
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;
            return new UserProfileResponse
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName!,
                PhoneNumber = user.PhoneNumber!,
                Role=role,
            };
        }

        public async Task<bool> IsUserNameUniqueAsync(string username, string role)
        {
            // FindByNameAsync ищет по NormalizedUserName — регистронезависимо
            return await _userManager.FindByNameAsync(username) is null;
        }

        public async Task<bool> IsPhoneUniqueAsync(string phone, string role)
        {
            var normalized = System.Text.RegularExpressions.Regex.Replace(phone, @"[\s\-\(\)]", "");
            return !await _userManager.Users
                                        .AnyAsync(u => u.PhoneNumber == normalized);

        }

        public async Task<bool> IsEmailUniqueAsync(string email, string role)
        {
            return await _userManager.FindByEmailAsync(email) is null;
        }

        public async Task<LoginResult> LoginUserByUserNameAsync(string username, string password)
        {
            // UserName в Identity нормализуется к upper-case, FindByNameAsync учитывает это
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return new LoginResult { Success = false, Token = string.Empty, Message = "Пользователь не найден" };

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
                return new LoginResult { Success = false, Token = string.Empty, Message = "Неверный пароль" };

            string jwt = await GenerateJwtToken(user);
            string refresh = GenerateRefreshToken();

            //user.RefreshToken = refresh;
            //user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new LoginResult { Success = true, Token = jwt, RefreshToken = refresh };
        }
        public async Task<UserProfileResponse> UpdateUserProfileAsync(UpdateUserProfileCommand request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                throw new ApplicationException("User not found.");
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.UserName = request.UserName;
            user.PhoneNumber = request.PhoneNumber;
            user.Email = request.Email;
            user.NormalizedEmail = request.Email?.ToUpperInvariant() ?? string.Empty;


            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new ApplicationException($"Failed to update profile: {result.Errors.Select(e => e.Description).FirstOrDefault()}");

            return new UserProfileResponse
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName!,
                PhoneNumber = user.PhoneNumber!,
                Role=role,
            };
        }


        public async Task<bool> ChangePasswordAsync(ChangePasswordCommand request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return false; 

            var changePasswordResult = await _userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
            
                throw new ApplicationException($"Password change failed: {changePasswordResult.Errors.Select(e => e.Description).FirstOrDefault()}");
            }

            return true;
        }
    }
}
