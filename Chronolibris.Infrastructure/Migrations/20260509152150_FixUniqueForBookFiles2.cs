using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUniqueForBookFiles2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_book_files_book_id_format_id_historical_text",
                table: "book_files");

            migrationBuilder.CreateIndex(
                name: "ix_book_files_book_id_format_id_historical_text",
                table: "book_files",
                columns: new[] { "book_id", "format_id", "historical_text" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_book_files_book_id_format_id_historical_text",
                table: "book_files");

            migrationBuilder.CreateIndex(
                name: "ix_book_files_book_id_format_id_historical_text",
                table: "book_files",
                columns: new[] { "book_id", "format_id", "historical_text" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", true);
        }
    }
}
