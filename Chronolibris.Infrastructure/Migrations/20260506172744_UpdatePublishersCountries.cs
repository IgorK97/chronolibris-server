using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePublishersCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "country_id",
                table: "publishers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "country_id",
                table: "publishers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "publishers",
                keyColumn: "id",
                keyValue: 1L,
                column: "country_id",
                value: 2L);

            migrationBuilder.UpdateData(
                table: "publishers",
                keyColumn: "id",
                keyValue: 2L,
                column: "country_id",
                value: 1L);
        }
    }
}
