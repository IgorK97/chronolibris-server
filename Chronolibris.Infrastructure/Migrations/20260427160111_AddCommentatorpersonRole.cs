using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentatorpersonRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "person_roles",
                columns: new[] { "id", "kind", "name" },
                values: new object[] { 10L, PersonRoleKind.Both, "Комментатор" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 10L);
        }
    }
}
