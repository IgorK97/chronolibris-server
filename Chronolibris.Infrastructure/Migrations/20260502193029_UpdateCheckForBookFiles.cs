using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCheckForBookFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_book_files_readable_format",
                table: "book_files");

            migrationBuilder.CreateIndex(
                name: "ix_book_files_book_id_format_id",
                table: "book_files",
                columns: new[] { "book_id", "format_id" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_book_files_readable_format",
                table: "book_files",
                sql: "NOT (\"is_readable\" = true) OR (\"format_id\" = 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_book_files_book_id_format_id",
                table: "book_files");

            migrationBuilder.DropCheckConstraint(
                name: "ck_book_files_readable_format",
                table: "book_files");

            migrationBuilder.AddCheckConstraint(
                name: "ck_book_files_readable_format",
                table: "book_files",
                sql: "NOT (\"is_readable\" = true) OR (\"format_id\" IN (1, 2))");
        }
    }
}
