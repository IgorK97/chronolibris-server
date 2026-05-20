using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChecksforStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_book_files_users_created_by",
                table: "book_files");

            migrationBuilder.DropForeignKey(
                name: "fk_books_users_created_by",
                table: "books");

            migrationBuilder.AlterColumn<long>(
                name: "created_by",
                table: "books",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "created_by",
                table: "book_files",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "book_file_statuses",
                columns: new[] { "id", "name" },
                values: new object[] { 7L, "Удален" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_selections_updated",
                table: "selections",
                sql: "(updated_at = NULL AND updated_by = NULL) OR (updated_at != NULL AND updated_by != NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_books_updated",
                table: "books",
                sql: "(updated_at = NULL AND updated_by = NULL) OR (updated_at != NULL AND updated_by != NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_book_files_completed",
                table: "book_files",
                sql: "(completed_at = NULL AND status_id < 4) OR (completed_at != NULL AND status_id >= 4)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_book_files_deleted",
                table: "book_files",
                sql: "(deleted_at = NULL AND status_id != 7) OR (deleted_at != NULL AND status_id = 7)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_book_files_hidden",
                table: "book_files",
                sql: "(hidden_at = NULL AND status_id < 6) OR (hidden_at != NULL AND status_id >=6)");

            migrationBuilder.AddForeignKey(
                name: "fk_book_files_users_created_by",
                table: "book_files",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_books_users_created_by",
                table: "books",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_book_files_users_created_by",
                table: "book_files");

            migrationBuilder.DropForeignKey(
                name: "fk_books_users_created_by",
                table: "books");

            migrationBuilder.DropCheckConstraint(
                name: "CK_selections_updated",
                table: "selections");

            migrationBuilder.DropCheckConstraint(
                name: "CK_books_updated",
                table: "books");

            migrationBuilder.DropCheckConstraint(
                name: "CK_book_files_completed",
                table: "book_files");

            migrationBuilder.DropCheckConstraint(
                name: "CK_book_files_deleted",
                table: "book_files");

            migrationBuilder.DropCheckConstraint(
                name: "CK_book_files_hidden",
                table: "book_files");

            migrationBuilder.DeleteData(
                table: "book_file_statuses",
                keyColumn: "id",
                keyValue: 7L);

            migrationBuilder.AlterColumn<long>(
                name: "created_by",
                table: "books",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "created_by",
                table: "book_files",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "fk_book_files_users_created_by",
                table: "book_files",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_books_users_created_by",
                table: "books",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
