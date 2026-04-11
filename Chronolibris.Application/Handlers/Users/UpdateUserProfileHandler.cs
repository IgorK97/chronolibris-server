using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Users;
using MediatR;

namespace Chronolibris.Application.Handlers.Users
{
    public class UpdateUserProfileHandler(IIdentityService identityService) : IRequestHandler<UpdateUserProfileCommand, UserProfileResponse>
    {
        public async Task<UserProfileResponse> Handle(UpdateUserProfileCommand command, CancellationToken ct)
        {
            return await identityService.UpdateUserProfileAsync(command);
        }
    }
}
