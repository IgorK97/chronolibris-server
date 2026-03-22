using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private const string AdminPasswordHash = "AQAAAAIAAYagAAAAEDJFJc162io4pjNy1E/Nf//bvX+ki234hGsZCcYkJjtPeR9CZQ1k/4T7Q2i+CWbPMg==";

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256);

            var dt = new DateTime(2025, 11, 20, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new User
                {
                    Id = 1,
                    LastName = "KQWERTY",
                    IsDeleted = false,
                    //LastEnteredAt = dt,
                    FirstName = "AQWERTY",
                    RegisteredAt = dt,
                    Email = "mail@mail.com",
                    NormalizedEmail = "MAIL@MAIL.COM",
                    UserName = "MainAdmin",
                    NormalizedUserName = "MAINADMIN",
                    EmailConfirmed = true,
                    SecurityStamp = "0d832e3a-efd3-490a-8572-c544467f8d83",
                    ConcurrencyStamp = "88d4f82e-f15b-4d84-8bba-6875af640148",
                    PasswordHash = AdminPasswordHash
                }
            );
        }
    }
}
