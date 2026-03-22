using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Requests;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    //public class RefreshTokenHandler(IIdentityService identityService) : IRequestHandler<RefreshTokenCommand, string>
    //{
    //    public async Task<string> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    //    {
    //        var newToken = await identityService.RefreshTokenAsync(request.token);
    //        if(newToken is null)
    //        {
    //            throw new UnauthorizedAccessException("Invalid token");
    //        }

    //        return newToken;
    //    }
    //}
}
