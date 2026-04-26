using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookmarkIndexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "para_index",
                table: "bookmarks");

            migrationBuilder.DropColumn(
                name: "max_para_index",
                table: "book_files");

            migrationBuilder.AddColumn<string>(
                name: "xpointer",
                table: "bookmarks",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xpointer",
                table: "bookmarks");

            migrationBuilder.AddColumn<int>(
                name: "para_index",
                table: "bookmarks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "max_para_index",
                table: "book_files",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
