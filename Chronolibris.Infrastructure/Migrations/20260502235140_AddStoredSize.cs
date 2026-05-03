using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "file_size_bytes",
                table: "book_files",
                newName: "stored_size");

            migrationBuilder.AddColumn<long>(
                name: "original_size",
                table: "book_files",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "original_size",
                table: "book_files");

            migrationBuilder.RenameColumn(
                name: "stored_size",
                table: "book_files",
                newName: "file_size_bytes");
        }
    }
}
