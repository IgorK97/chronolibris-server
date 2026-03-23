using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Chronolibris.Infrastructure.Data
{
    public class User : IdentityUser<long>
    {
        //[Key]
        //public required long Id { get; set; }
        //public required string Email { get; set; }
        [MaxLength(256)]
        public required string FirstName { get; set; }
        //public required string Password { get; set; }
        [MaxLength(256)]
        public required string LastName { get; set; }
        public required DateTime RegisteredAt { get; set; }
        //public required DateTime LastEnteredAt { get; set; }
        public required bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        //public string? RefreshToken { get; set; }
        //public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
