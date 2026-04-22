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
        [MaxLength(255)]
        public required string FirstName { get; set; }
        [MaxLength(255)]
        public required string LastName { get; set; }
        public required DateTime RegisteredAt { get; set; }
        public required bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}
