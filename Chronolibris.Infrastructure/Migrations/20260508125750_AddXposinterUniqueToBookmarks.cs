using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddXposinterUniqueToBookmarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_bookmarks_user_id",
                table: "bookmarks");

            migrationBuilder.CreateIndex(
                name: "uq_bookmark_user_book_position",
                table: "bookmarks",
                columns: new[] { "user_id", "book_file_id", "xpointer" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_bookmark_user_book_position",
                table: "bookmarks");

            migrationBuilder.CreateIndex(
                name: "ix_bookmarks_user_id",
                table: "bookmarks",
                column: "user_id");
        }
    }
}
