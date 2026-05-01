using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameUnknownToOther : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 22L,
                column: "name",
                value: "Другое");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 22L,
                column: "name",
                value: "Неизвестно");
        }
    }
}
