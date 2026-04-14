using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Users
{
    public class RegisterStaffCommand : IRequest<RegistrationResult>
    {
        [MaxLength(256)]
        public required string UserName { get; set; }
        [MaxLength(256)]
        public required string LastName { get; set; }
        [MaxLength(256)]
        public required string FirstName { get; set; }
        [MaxLength(256)]
        public required string Email { get; set; }
        [MaxLength(20)]
        public required string PhoneNumber { get; set; }
        [MaxLength(128)]
        public required string Password { get; set; }
        public required string Role { get; set; }
    }

}
