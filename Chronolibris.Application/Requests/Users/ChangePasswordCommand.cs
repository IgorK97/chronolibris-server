using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.Users
{
    public class ChangePasswordCommand : IRequest<Unit>
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public long UserId { get; set; }
    }
}
