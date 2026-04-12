using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Users;
using Chronolibris.Domain.Exceptions;
using MediatR;

namespace Chronolibris.Application.Handlers.Users
{

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
                throw new ChronolibrisException("Недопустимая роль", ErrorType.Validation);

            //if (!await _identityService.IsUserNameUniqueAsync(request.UserName))
            //    throw new ChronolibrisException("Такое имя пользователя уже используется", ErrorType.Conflict);

            //if (!await _identityService.IsEmailUniqueAsync(request.Email))
            //    throw new ChronolibrisException("Такой адрес электронной почты уже занят", ErrorType.Conflict);

            //if (!await _identityService.IsPhoneUniqueAsync(request.PhoneNumber!))
            //    throw new ChronolibrisException("Такой номер телефона уже занят", ErrorType.Conflict);

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
