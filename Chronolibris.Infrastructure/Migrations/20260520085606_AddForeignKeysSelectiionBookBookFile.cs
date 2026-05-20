using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeysSelectiionBookBookFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_selections_users_user_id",
                table: "selections");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "comments");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "selections",
                newName: "created_by");

            migrationBuilder.RenameIndex(
                name: "ix_selections_user_id",
                table: "selections",
                newName: "ix_selections_created_by");

            migrationBuilder.AddColumn<long>(
                name: "updated_by",
                table: "selections",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "created_by",
                table: "books",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "updated_by",
                table: "books",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "created_by",
                table: "book_files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "book_files",
                type: "timestamp with time zone",
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

            migrationBuilder.CreateIndex(
                name: "ix_books_created_by",
                table: "books",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_books_updated_by",
                table: "books",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "ix_book_files_created_by",
                table: "book_files",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_book_files_deleted_by",
                table: "book_files",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_book_files_hidden_by",
                table: "book_files",
                column: "hidden_by");

            migrationBuilder.AddForeignKey(
                name: "fk_book_files_users_created_by",
                table: "book_files",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id");

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
                name: "fk_books_users_created_by",
                table: "books",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_books_users_updated_by",
                table: "books",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_selections_users_created_by",
                table: "selections",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_selections_users_updated_by",
                table: "selections",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_book_files_users_created_by",
                table: "book_files");

            migrationBuilder.DropForeignKey(
                name: "fk_book_files_users_deleted_by",
                table: "book_files");

            migrationBuilder.DropForeignKey(
                name: "fk_book_files_users_hidden_by",
                table: "book_files");

            migrationBuilder.DropForeignKey(
                name: "fk_books_users_created_by",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "fk_books_users_updated_by",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "fk_selections_users_created_by",
                table: "selections");

            migrationBuilder.DropForeignKey(
                name: "fk_selections_users_updated_by",
                table: "selections");

            migrationBuilder.DropIndex(
                name: "ix_selections_updated_by",
                table: "selections");

            migrationBuilder.DropIndex(
                name: "ix_books_created_by",
                table: "books");

            migrationBuilder.DropIndex(
                name: "ix_books_updated_by",
                table: "books");

            migrationBuilder.DropIndex(
                name: "ix_book_files_created_by",
                table: "book_files");

            migrationBuilder.DropIndex(
                name: "ix_book_files_deleted_by",
                table: "book_files");

            migrationBuilder.DropIndex(
                name: "ix_book_files_hidden_by",
                table: "book_files");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "selections");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "books");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "books");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "book_files");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "book_files");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                table: "book_files");

            migrationBuilder.DropColumn(
                name: "hidden_by",
                table: "book_files");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "selections",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_selections_created_by",
                table: "selections",
                newName: "ix_selections_user_id");

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "comments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "fk_selections_users_user_id",
                table: "selections",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
