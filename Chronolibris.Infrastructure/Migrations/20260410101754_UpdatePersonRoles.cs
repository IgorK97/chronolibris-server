using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePersonRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 10L);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "kind",
                value: PersonRoleKind.Content);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 2L,
                column: "kind",
                value: PersonRoleKind.Book);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 3L,
                column: "kind",
                value: PersonRoleKind.Book);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 5L,
                column: "kind",
                value: PersonRoleKind.Book);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 6L,
                column: "kind",
                value: PersonRoleKind.Book);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 7L,
                column: "kind",
                value: PersonRoleKind.Book);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 8L,
                column: "kind",
                value: PersonRoleKind.Book);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 9L,
                columns: new[] { "kind", "name" },
                values: new object[] { PersonRoleKind.Content, "Адресат" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 2L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 3L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 5L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 6L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 7L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 8L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 9L,
                columns: new[] { "kind", "name" },
                values: new object[] { PersonRoleKind.Both, "Комментатор" });

            migrationBuilder.InsertData(
                table: "person_roles",
                columns: new[] { "id", "name" },
                values: new object[] { 10L, "Адресат" });
        }
    }
}
