using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSomeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 12L);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 10L,
                column: "name",
                value: "Адресат");

            migrationBuilder.InsertData(
                table: "person_roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 8L, "Редактор перевода" },
                    { 9L, "Комментатор" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 9L);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 10L,
                column: "name",
                value: "Редактор перевода");

            migrationBuilder.InsertData(
                table: "person_roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 11L, "Комментатор" },
                    { 12L, "Адресат" }
                });
        }
    }
}
