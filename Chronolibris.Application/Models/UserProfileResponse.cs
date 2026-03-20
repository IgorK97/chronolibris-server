using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Application.Models
{
     public class UserProfileResponse
    {
        public long UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Email { get; set; }
        public required string UserName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
    }
}
