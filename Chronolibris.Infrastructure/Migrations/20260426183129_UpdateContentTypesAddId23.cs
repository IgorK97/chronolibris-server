using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContentTypesAddId23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 13L,
                column: "name",
                value: "Религиозный трактат");

            migrationBuilder.InsertData(
                table: "content_type",
                columns: new[] { "id", "name", "nature" },
                values: new object[] { 24L, "Священный текст", ContentNature.Document });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 24L);

            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 13L,
                column: "name",
                value: "Богословский трактат");
        }
    }
}
