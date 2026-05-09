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
        public void Configure(EntityTypeBuilder<User> builder)
        {

            builder.Property(user => user.PhoneNumber)
                .IsUnicode(false)
                .IsFixedLength(false)
                .HasMaxLength(20);

            builder.Property(user => user.PasswordHash)
                .IsUnicode(false)
                .IsFixedLength(true)
                .HasMaxLength(256); //84

            builder.Property(user => user.ConcurrencyStamp)
                .IsUnicode(false)
                .IsFixedLength(true)
                .HasMaxLength(36)
                .IsRequired(true);

            builder.Property(user => user.SecurityStamp)
                .IsUnicode(false)
                .IsFixedLength(false)
                .HasMaxLength(36)
                .IsRequired(true);

            builder
                .Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.RegisteredAt).HasDefaultValueSql("CURRENT_TIMESTAMP");


            builder.ToTable(t => t.HasCheckConstraint(
                "ck_users_first_name_correct_length",
                "LENGTH(TRIM(first_name))>0 AND LENGTH(TRIM(first_name))<65"));

            builder.ToTable(t => t.HasCheckConstraint(
                "ck_users_last_name_correct_length",
                "LENGTH(TRIM(last_name))>0 AND LENGTH(TRIM(last_name))<65"));

            builder.ToTable(t => t.HasCheckConstraint(
                "ck_users_user_name_correct_length",
                "LENGTH(TRIM(user_name))>4 AND LENGTH(TRIM(user_name))<33"));

            //var dt = new DateTime(2025, 11, 20, 0, 0, 0, DateTimeKind.Utc);

            //builder.HasData(
            //    new User
            //    {
            //        Id = 1,
            //        LastName = "KQWERTY",
            //        IsDeleted = false,
            //        //LastEnteredAt = dt,
            //        FirstName = "AQWERTY",
            //        RegisteredAt = dt,
            //        Email = "mail@mail.com",
            //        NormalizedEmail = "MAIL@MAIL.COM",
            //        UserName = "MainAdmin",
            //        NormalizedUserName = "MAINADMIN",
            //        EmailConfirmed = true,
            //        SecurityStamp = "0d832e3a-efd3-490a-8572-c544467f8d83",
            //        ConcurrencyStamp = "88d4f82e-f15b-4d84-8bba-6875af640148",
            //        PasswordHash = AdminPasswordHash
            //    }
            //);
        }
    }
}
