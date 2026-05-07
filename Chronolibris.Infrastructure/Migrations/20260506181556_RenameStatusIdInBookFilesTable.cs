using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameStatusIdInBookFilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_book_files_book_file_statuses_book_file_status_id",
                table: "book_files");

            migrationBuilder.RenameColumn(
                name: "book_file_status_id",
                table: "book_files",
                newName: "status_id");

            migrationBuilder.RenameIndex(
                name: "ix_book_files_book_file_status_id",
                table: "book_files",
                newName: "ix_book_files_status_id");

            migrationBuilder.AddForeignKey(
                name: "fk_book_files_book_file_statuses_status_id",
                table: "book_files",
                column: "status_id",
                principalTable: "book_file_statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_book_files_book_file_statuses_status_id",
                table: "book_files");

            migrationBuilder.RenameColumn(
                name: "status_id",
                table: "book_files",
                newName: "book_file_status_id");

            migrationBuilder.RenameIndex(
                name: "ix_book_files_status_id",
                table: "book_files",
                newName: "ix_book_files_book_file_status_id");

            migrationBuilder.AddForeignKey(
                name: "fk_book_files_book_file_statuses_book_file_status_id",
                table: "book_files",
                column: "book_file_status_id",
                principalTable: "book_file_statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
