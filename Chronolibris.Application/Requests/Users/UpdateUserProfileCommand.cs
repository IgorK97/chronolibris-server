using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Users
{
    public class UpdateUserProfileCommand : IRequest<UserProfileResponse>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Email { get; set; }
        public long UserId { get; set; } 
        public string? PhoneNumber { get; set; }
        public required string UserName { get; set; }
    }
}
