using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRootContentIdForContentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "root_content_id",
                table: "contents",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "contents",
                keyColumn: "id",
                keyValue: 1L,
                column: "root_content_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "contents",
                keyColumn: "id",
                keyValue: 2L,
                column: "root_content_id",
                value: null);

            migrationBuilder.CreateIndex(
                name: "ix_contents_root_content_id",
                table: "contents",
                column: "root_content_id");

            migrationBuilder.AddForeignKey(
                name: "fk_contents_contents_root_content_id",
                table: "contents",
                column: "root_content_id",
                principalTable: "contents",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_contents_contents_root_content_id",
                table: "contents");

            migrationBuilder.DropIndex(
                name: "ix_contents_root_content_id",
                table: "contents");

            migrationBuilder.DropColumn(
                name: "root_content_id",
                table: "contents");
        }
    }
}
