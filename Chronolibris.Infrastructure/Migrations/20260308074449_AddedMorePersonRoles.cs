using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedMorePersonRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "person_roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 4L, "Иллюстратор" },
                    { 5L, "Составитель" },
                    { 6L, "Корректор" },
                    { 7L, "Научный редактор" },
                    { 8L, "Литературный редактор" },
                    { 9L, "Технический редактор" },
                    { 10L, "Редактор перевода" },
                    { 11L, "Оцифровщик" },
                    { 12L, "Автор предисловия" },
                    { 13L, "Автор послесловия" },
                    { 14L, "Комментатор" },
                    { 15L, "Дизайнер" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 15L);
        }
    }
}
