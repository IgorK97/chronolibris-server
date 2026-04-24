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
using Chronolibris.Application.Requests.Users;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PhoneNumbers;
using ErrorType = Chronolibris.Domain.Exceptions.ErrorType;

namespace Chronolibris.Infrastructure.Services.IdentityService
{

    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext dbContext;

        public IdentityService(UserManager<User> userManager, 
            SignInManager<User> signInManager,
            IConfiguration config,
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            dbContext = applicationDbContext;
        }
        public async Task<RegistrationResult> RegisterUserAsync(RegisterRequest request)
        {
            DateTime dt = DateTime.UtcNow;
            var util = PhoneNumberUtil.GetInstance();
            var number = util.Parse(request.PhoneNumber, "RU");
            bool isValid = util.IsValidNumber(number);
            if (!isValid)
                throw new ChronolibrisException("Невалидный номер телефона", ErrorType.Validation);

            var existingUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);

            if (existingUser != null)
                throw new ChronolibrisException("Номер телефона уже занят", ErrorType.Conflict);

            var user = new User
            {
                LastName = request.LastName,
                IsDeleted = false,
                //LastEnteredAt = dt,
                FirstName = request.FirstName,
                RegisteredAt = dt,
                Email = request.Email,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new ChronolibrisException(string.Join(", ", result.Errors.Select(e => e.Description)), ErrorType.Conflict);
            };

            var role = string.IsNullOrWhiteSpace(request.Role) ? "reader" : request.Role;

            await _userManager.AddToRoleAsync(user, role);

            //var refreshToken = GenerateRefreshToken();
            //user.RefreshToken = refreshToken;
            //user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(365);
           
            await _userManager.UpdateAsync(user);
            return new RegistrationResult
            {
                Success = result.Succeeded,
                UserId = user.Id,
                Token = await GenerateJwtToken(user),
                //RefreshToken = refreshToken,
                Message = result.Succeeded ? null : result.Errors.Select(e => e.Description).FirstOrDefault()
            };
        
        }

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
                //UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName!,
                PhoneNumber = user.PhoneNumber!,
                Role=role,
            };
        }

        public async Task<LoginResult> LoginUserByUserNameAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                throw new ChronolibrisException("Неверный логин или пароль", ErrorType.NotFound);

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
                throw new ChronolibrisException("Неверный логин или пароль", ErrorType.NotFound);


            string jwt = await GenerateJwtToken(user);
            //string refresh = GenerateRefreshToken();

            //user.RefreshToken = refresh;
            //user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            //await _userManager.UpdateAsync(user);

            return new LoginResult { Success = true, Token = jwt };
        }
        public async Task<UserProfileResponse> UpdateUserProfileAsync(UpdateUserProfileCommand request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                throw new ChronolibrisException("Пользователь не найден", ErrorType.NotFound);
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
                throw new ChronolibrisException($"Не удалось обновить профиль: {result.Errors.Select(e => e.Description).FirstOrDefault()}", ErrorType.Conflict);

            return new UserProfileResponse
            {
                //UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName!,
                PhoneNumber = user.PhoneNumber!,
                Role=role,
            };
        }


        public async Task ChangePasswordAsync(ChangePasswordCommand request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                throw new ChronolibrisException("Пользователь не найден", ErrorType.NotFound);

            var changePasswordResult = await _userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                //throw new ChronolibrisException("Пользователь не найден или неправильный пароль", ErrorType.NotFound);
                //throw new ChronolibrisException($"Ошибка: {changePasswordResult.Errors.Select(e => e.Description).FirstOrDefault()}", ErrorType.Conflict);
                throw new ChronolibrisException(string.Join(", ", changePasswordResult.Errors.Select(e => e.Description)), ErrorType.Validation);

            }
        }

        public async Task<bool> IsUserActiveAsync(long userId)
        {
            var user = await dbContext.Users
            .FromSqlRaw("SELECT * FROM users WHERE id = {0} FOR UPDATE", userId)
            .AsNoTracking()
            .FirstOrDefaultAsync();
            //var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null && !user.IsDeleted;
        }
    }
}
