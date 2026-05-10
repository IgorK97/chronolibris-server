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
