using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class RoleConfiguration :  IEntityTypeConfiguration<IdentityRole<long>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<long>> builder)
        {
            builder.HasData(
                new IdentityRole<long>
                {
                    Id = 1,
                    Name = "admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole<long>
                {
                    Id = 2,
                    Name = "reader",
                    NormalizedName = "READER"
                },
                new IdentityRole<long>
                {
                    Id=3,
                    Name="moderator",
                    NormalizedName="MODERATOR"
                }
            );
        }
    }
}
