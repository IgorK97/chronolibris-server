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
    public class GetUserProfileHandler(IIdentityService identityService) : IRequestHandler<GetUserProfileQuery, UserProfileResponse?>
    {
        public async Task<UserProfileResponse?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            return await identityService.GetUserProfileAsync(request.userId);

        }
    }
}
