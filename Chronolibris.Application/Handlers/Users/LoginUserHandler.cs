using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Interfaces;
using MediatR;
using Chronolibris.Application.Requests.Users;

namespace Chronolibris.Application.Handlers.Users
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResult>
    {
        private readonly IIdentityService _identityService;
        public LoginUserHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<LoginResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.LoginUserByUserNameAsync(request.UserName, request.Password);
            return result;
        }
    }
}
