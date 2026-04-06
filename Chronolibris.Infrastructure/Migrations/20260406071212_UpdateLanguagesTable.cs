using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLanguagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code",
                table: "languages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "languages",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "languages",
                keyColumn: "id",
                keyValue: 1L,
                column: "code",
                value: "english");

            migrationBuilder.UpdateData(
                table: "languages",
                keyColumn: "id",
                keyValue: 2L,
                column: "code",
                value: "russian");

            migrationBuilder.UpdateData(
                table: "languages",
                keyColumn: "id",
                keyValue: 3L,
                column: "code",
                value: "french");

            migrationBuilder.UpdateData(
                table: "languages",
                keyColumn: "id",
                keyValue: 4L,
                column: "code",
                value: "german");
        }
    }
}
