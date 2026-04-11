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
    public class ChangePasswordHandler(IIdentityService identityService) : IRequestHandler<ChangePasswordCommand, Unit>
    {
        public async Task<Unit> Handle(ChangePasswordCommand command, CancellationToken ct)
        {
            await identityService.ChangePasswordAsync(command);
            return Unit.Value;
        }
    }
}
