using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteSomeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 9L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "person_roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 8L, "Литературный редактор" },
                    { 9L, "Технический редактор" }
                });
        }
    }
}
