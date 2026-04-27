using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYearToFieldForContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "year",
                table: "contents",
                newName: "year_to");

            migrationBuilder.AddColumn<int>(
                name: "year_from",
                table: "contents",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "contents",
                keyColumn: "id",
                keyValue: 1L,
                column: "year_from",
                value: 1993);

            migrationBuilder.UpdateData(
                table: "contents",
                keyColumn: "id",
                keyValue: 2L,
                column: "year_from",
                value: 1979);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "year_from",
                table: "contents");

            migrationBuilder.RenameColumn(
                name: "year_to",
                table: "contents",
                newName: "year");
        }
    }
}
