using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Users
{
    public class RegisterStaffCommand : IRequest<RegistrationResult>
    {
        public string UserName { get; set; } = string.Empty;
        public required string LastName { get; set; }
        public required string FirstName { get; set; }
        public string Email { get; set; } = string.Empty;
        public required string PhoneNumber { get; set; }

        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class RegisterStaffHandler : IRequestHandler<RegisterStaffCommand, RegistrationResult>
    {
        private readonly IIdentityService _identityService;
        private static readonly HashSet<string> AllowedRoles =
            new(StringComparer.OrdinalIgnoreCase) { "moderator", "admin" };

        public RegisterStaffHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<RegistrationResult> Handle(
            RegisterStaffCommand request, CancellationToken ct)
        {
            if (!AllowedRoles.Contains(request.Role))
                return new RegistrationResult
                {
                    UserId= 0,
                    Success = false,
                    Message = $"Недопустимая роль «{request.Role}». Допустимые: Moderator, Admin.",
                };
            if (!await _identityService.IsUserNameUniqueAsync(request.UserName))
                return new RegistrationResult
                {
                    UserId=0,
                    Success = false,
                    Message = "Это имя пользователя уже занято.",
                };

            if (!await _identityService.IsEmailUniqueAsync(request.Email))
                return new RegistrationResult {UserId=0, Success = false, Message = "Этот email уже зарегистрирован." };

            if (!await _identityService.IsPhoneUniqueAsync(request.PhoneNumber!))
                return new RegistrationResult { UserId=0, Success = false, Message = "Этот номер телефона уже зарегистрирован." };
            return await _identityService.RegisterUserAsync(new RegisterRequest
            {
            
                UserName = request.UserName,
                LastName = request.LastName,
                FirstName = request.FirstName,
                Email = request.Email,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
            });
        }
    }
}
