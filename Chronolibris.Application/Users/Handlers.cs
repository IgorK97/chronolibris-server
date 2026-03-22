using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Users
{
    public class RegisterStaffCommand : IRequest<RegistrationResult>
    {
        public string UserName { get; set; } = string.Empty;
        public required string LastName { get; set; }
        public required string FirstName { get; set; }
        public string Email { get; set; } = string.Empty;
        public required string PhoneNumber { get; set; }

        public string Password { get; set; } = string.Empty;
        /// <summary>"Moderator" или "Admin"</summary>
        public string Role { get; set; } = string.Empty;
    }

    // ── Хендлер ───────────────────────────────────────────────────────────────

    public class RegisterStaffHandler : IRequestHandler<RegisterStaffCommand, RegistrationResult>
    {
        private readonly IIdentityService _identityService;

        // Роли, которые разрешено назначать через этот хендлер.
        // Reader сюда не входит — для него отдельный RegisterUserHandler.
        private static readonly HashSet<string> AllowedRoles =
            new(StringComparer.OrdinalIgnoreCase) { "moderator", "admin" };

        public RegisterStaffHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<RegistrationResult> Handle(
            RegisterStaffCommand request, CancellationToken ct)
        {
            // ── Валидация роли ────────────────────────────────────────────────
            if (!AllowedRoles.Contains(request.Role))
                return new RegistrationResult
                {
                    UserId= 0,
                    Success = false,
                    Message = $"Недопустимая роль «{request.Role}». Допустимые: Moderator, Admin.",
                };

            // ── Проверка уникальности username (глобально) ────────────────────
            if (!await _identityService.IsUserNameUniqueAsync(request.UserName))
                return new RegistrationResult
                {
                    UserId=0,
                    Success = false,
                    Message = "Это имя пользователя уже занято.",
                };

            // ── Проверка уникальности email (глобально) ───────────────────────
            if (!string.IsNullOrEmpty(request.Email) &&
                !await _identityService.IsEmailUniqueAsync(request.Email))
                return new RegistrationResult
                {
                    UserId=0,
                    Success = false,
                    Message = "Этот email уже зарегистрирован.",
                };

            // ── Проверка уникальности телефона (глобально) ────────────────────
            if (!string.IsNullOrEmpty(request.PhoneNumber) &&
                !await _identityService.IsPhoneUniqueAsync(request.PhoneNumber))
                return new RegistrationResult
                {
                    UserId=0,
                    Success = false,
                    Message = "Этот номер телефона уже зарегистрирован.",
                };
            // ── Регистрация с нужной ролью ────────────────────────────────────
            return await _identityService.RegisterUserAsync(new RegisterRequest
            {
            
                UserName = request.UserName,
                LastName = request.LastName,
                FirstName = request.FirstName,
                Email = request.Email,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,   // передаём роль в сервис
            });
        }
    }
}
