using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX uix_users_phone
                ON users(phone_number)
                WHERE phone_number IS NOT NULL;
            ");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "users");


            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX ""EmailIndex""
                ON public.users (normalized_email);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS uix_users_phone;");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""EmailIndex"";");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "users",
                column: "normalized_email");
        }
    }
}
