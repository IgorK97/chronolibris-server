using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHasDataForUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "users",
            //    keyColumn: "id",
            //    keyValue: 1L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.InsertData(
                //table: "users",
                //columns: new[] { "id", "access_failed_count", "concurrency_stamp", "deleted_at", "email", "email_confirmed", "first_name", "is_deleted", "last_name", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "registered_at", "security_stamp", "two_factor_enabled", "user_name" },
                //values: new object[] { 1L, 0, "88d4f82e-f15b-4d84-8bba-6875af640148", null, "mail@mail.com", true, "AQWERTY", false, "KQWERTY", false, null, "MAIL@MAIL.COM", "MAINADMIN", "AQAAAAIAAYagAAAAEDJFJc162io4pjNy1E/Nf//bvX+ki234hGsZCcYkJjtPeR9CZQ1k/4T7Q2i+CWbPMg==", null, false, new DateTime(2025, 11, 20, 0, 0, 0, 0, DateTimeKind.Utc), "0d832e3a-efd3-490a-8572-c544467f8d83", false, "MainAdmin" });
        }
    }
}
