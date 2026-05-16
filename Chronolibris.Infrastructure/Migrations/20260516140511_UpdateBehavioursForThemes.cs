using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBehavioursForThemes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_books_publishers_publisher_id",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "fk_content_theme_themes_theme_id",
                table: "content_theme");

            migrationBuilder.AddForeignKey(
                name: "fk_books_publishers_publisher_id",
                table: "books",
                column: "publisher_id",
                principalTable: "publishers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_content_theme_themes_theme_id",
                table: "content_theme",
                column: "theme_id",
                principalTable: "themes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_books_publishers_publisher_id",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "fk_content_theme_themes_theme_id",
                table: "content_theme");

            migrationBuilder.AddForeignKey(
                name: "fk_books_publishers_publisher_id",
                table: "books",
                column: "publisher_id",
                principalTable: "publishers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_content_theme_themes_theme_id",
                table: "content_theme",
                column: "theme_id",
                principalTable: "themes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
