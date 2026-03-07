using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRegConfigToLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "fts_configuration",
                table: "languages",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "languages",
                keyColumn: "id",
                keyValue: 1L,
                column: "fts_configuration",
                value: "english");

            migrationBuilder.UpdateData(
                table: "languages",
                keyColumn: "id",
                keyValue: 2L,
                column: "fts_configuration",
                value: "russian");

            migrationBuilder.UpdateData(
                table: "languages",
                keyColumn: "id",
                keyValue: 3L,
                column: "fts_configuration",
                value: "french");

            migrationBuilder.UpdateData(
                table: "languages",
                keyColumn: "id",
                keyValue: 4L,
                column: "fts_configuration",
                value: "german");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fts_configuration",
                table: "languages");
        }
    }
}
