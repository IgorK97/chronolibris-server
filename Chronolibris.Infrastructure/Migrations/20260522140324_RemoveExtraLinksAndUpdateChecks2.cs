using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraLinksAndUpdateChecks2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_book_files_users_deleted_by",
                table: "book_files");

            migrationBuilder.DropForeignKey(
                name: "fk_book_files_users_hidden_by",
                table: "book_files");

            migrationBuilder.DropForeignKey(
                name: "fk_books_users_updated_by",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "fk_selections_users_updated_by",
                table: "selections");

            migrationBuilder.DropIndex(
                name: "ix_selections_updated_by",
                table: "selections");

            migrationBuilder.DropCheckConstraint(
                name: "CK_selections_updated",
                table: "selections");

            migrationBuilder.DropIndex(
                name: "ix_books_updated_by",
                table: "books");

            migrationBuilder.DropCheckConstraint(
                name: "CK_books_updated",
                table: "books");

            migrationBuilder.DropIndex(
                name: "ix_book_files_deleted_by",
                table: "book_files");

            migrationBuilder.DropIndex(
                name: "ix_book_files_hidden_by",
                table: "book_files");

            migrationBuilder.DropCheckConstraint(
                name: "CK_book_files_completed",
                table: "book_files");

            migrationBuilder.DropCheckConstraint(
                name: "CK_book_files_deleted",
                table: "book_files");

            migrationBuilder.DropCheckConstraint(
                name: "CK_book_files_hidden",
                table: "book_files");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "selections");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "books");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                table: "book_files");

            migrationBuilder.DropColumn(
                name: "hidden_by",
                table: "book_files");

            migrationBuilder.AddCheckConstraint(
                name: "CK_book_files_completed",
                table: "book_files",
                sql: "(completed_at is NULL AND status_id < 4) OR (completed_at is NOT NULL AND status_id >= 4)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_book_files_deleted",
                table: "book_files",
                sql: "(deleted_at is NULL AND status_id != 7) OR (deleted_at is NOT NULL AND status_id = 7)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_book_files_hidden",
                table: "book_files",
                sql: "(hidden_at is NULL AND status_id < 6) OR (hidden_at is NOT NULL AND status_id >=6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_book_files_completed",
                table: "book_files");

            migrationBuilder.DropCheckConstraint(
                name: "CK_book_files_deleted",
                table: "book_files");

            migrationBuilder.DropCheckConstraint(
                name: "CK_book_files_hidden",
                table: "book_files");

            migrationBuilder.AddColumn<long>(
                name: "updated_by",
                table: "selections",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "updated_by",
                table: "books",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "deleted_by",
                table: "book_files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "hidden_by",
                table: "book_files",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 1L,
                column: "updated_by",
                value: null);

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 2L,
                column: "updated_by",
                value: null);

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 3L,
                column: "updated_by",
                value: null);

            migrationBuilder.CreateIndex(
                name: "ix_selections_updated_by",
                table: "selections",
                column: "updated_by");

            migrationBuilder.AddCheckConstraint(
                name: "CK_selections_updated",
                table: "selections",
                sql: "(updated_at = NULL AND updated_by = NULL) OR (updated_at != NULL AND updated_by != NULL)");

            migrationBuilder.CreateIndex(
                name: "ix_books_updated_by",
                table: "books",
                column: "updated_by");

            migrationBuilder.AddCheckConstraint(
                name: "CK_books_updated",
                table: "books",
                sql: "(updated_at = NULL AND updated_by = NULL) OR (updated_at != NULL AND updated_by != NULL)");

            migrationBuilder.CreateIndex(
                name: "ix_book_files_deleted_by",
                table: "book_files",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_book_files_hidden_by",
                table: "book_files",
                column: "hidden_by");

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
                name: "fk_book_files_users_deleted_by",
                table: "book_files",
                column: "deleted_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_book_files_users_hidden_by",
                table: "book_files",
                column: "hidden_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_books_users_updated_by",
                table: "books",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_selections_users_updated_by",
                table: "selections",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
