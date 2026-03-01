using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBookContentAddContentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_books_books_parent_book_id",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "fk_contents_contents_parent_content_id",
                table: "contents");

            migrationBuilder.DropForeignKey(
                name: "fk_contents_contents_root_content_id",
                table: "contents");

            migrationBuilder.DropIndex(
                name: "ix_contents_root_content_id",
                table: "contents");

            migrationBuilder.DropIndex(
                name: "ix_books_parent_book_id",
                table: "books");

            migrationBuilder.DropColumn(
                name: "is_original",
                table: "contents");

            migrationBuilder.DropColumn(
                name: "is_translate",
                table: "contents");

            migrationBuilder.DropColumn(
                name: "root_content_id",
                table: "contents");

            migrationBuilder.DropColumn(
                name: "parent_book_id",
                table: "books");

            migrationBuilder.RenameColumn(
                name: "uploaded_at",
                table: "digital_files",
                newName: "updated_at");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "digital_files",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<bool>(
                name: "is_readable",
                table: "digital_files",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "position",
                table: "contents",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<long>(
                name: "content_type_id",
                table: "contents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "is_reviewable",
                table: "books",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "content_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nature = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_type", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "id",
                keyValue: 1L,
                column: "is_reviewable",
                value: true);

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "id",
                keyValue: 2L,
                column: "is_reviewable",
                value: true);

            migrationBuilder.InsertData(
                table: "content_type",
                columns: new[] { "id", "name", "nature" },
                values: new object[,]
                {
                    { 1L, "Дневник", "Document" },
                    { 2L, "Письмо", "Document" },
                    { 3L, "Мемуары", "Document" },
                    { 4L, "Автобиография", "Document" },
                    { 5L, "Хроника", "Document" },
                    { 6L, "Летопись", "Document" },
                    { 7L, "Манифест", "Document" },
                    { 8L, "Речь", "Document" },
                    { 9L, "Указ", "Document" },
                    { 10L, "Рассказ", "Work" },
                    { 11L, "Роман", "Work" },
                    { 12L, "Философский трактат", "Work" },
                    { 13L, "Богословский трактат", "Work" },
                    { 14L, "Политический трактат", "Work" },
                    { 15L, "Биография", "Work" },
                    { 16L, "Путевые заметки", "Work" },
                    { 17L, "Сборник", "Work" },
                    { 18L, "Учебник", "Work" },
                    { 19L, "Историческое исследование", "Analysis" },
                    { 20L, "Монография", "Analysis" },
                    { 21L, "Научная статья", "Analysis" },
                    { 22L, "Неизвестно", "Unknown" }
                });

            migrationBuilder.UpdateData(
                table: "contents",
                keyColumn: "id",
                keyValue: 1L,
                column: "content_type_id",
                value: 20L);

            migrationBuilder.UpdateData(
                table: "contents",
                keyColumn: "id",
                keyValue: 2L,
                column: "content_type_id",
                value: 19L);

            migrationBuilder.Sql(
                "UPDATE contents SET content_type_id = 22 WHERE content_type_id = 0 OR content_type_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_contents_content_type_id",
                table: "contents",
                column: "content_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_contents_content_type_content_type_id",
                table: "contents",
                column: "content_type_id",
                principalTable: "content_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_contents_contents_parent_content_id",
                table: "contents",
                column: "parent_content_id",
                principalTable: "contents",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_contents_content_type_content_type_id",
                table: "contents");

            migrationBuilder.DropForeignKey(
                name: "fk_contents_contents_parent_content_id",
                table: "contents");

            migrationBuilder.DropTable(
                name: "content_type");

            migrationBuilder.DropIndex(
                name: "ix_contents_content_type_id",
                table: "contents");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "digital_files");

            migrationBuilder.DropColumn(
                name: "is_readable",
                table: "digital_files");

            migrationBuilder.DropColumn(
                name: "content_type_id",
                table: "contents");

            migrationBuilder.DropColumn(
                name: "is_reviewable",
                table: "books");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "digital_files",
                newName: "uploaded_at");

            migrationBuilder.AlterColumn<int>(
                name: "position",
                table: "contents",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_original",
                table: "contents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_translate",
                table: "contents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "root_content_id",
                table: "contents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "parent_book_id",
                table: "books",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "id",
                keyValue: 1L,
                column: "parent_book_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "id",
                keyValue: 2L,
                column: "parent_book_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "contents",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "is_original", "is_translate", "root_content_id" },
                values: new object[] { true, false, null });

            migrationBuilder.UpdateData(
                table: "contents",
                keyColumn: "id",
                keyValue: 2L,
                columns: new[] { "is_original", "is_translate", "root_content_id" },
                values: new object[] { false, true, null });

            migrationBuilder.CreateIndex(
                name: "ix_contents_root_content_id",
                table: "contents",
                column: "root_content_id");

            migrationBuilder.CreateIndex(
                name: "ix_books_parent_book_id",
                table: "books",
                column: "parent_book_id");

            migrationBuilder.AddForeignKey(
                name: "fk_books_books_parent_book_id",
                table: "books",
                column: "parent_book_id",
                principalTable: "books",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_contents_contents_parent_content_id",
                table: "contents",
                column: "parent_content_id",
                principalTable: "contents",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_contents_contents_root_content_id",
                table: "contents",
                column: "root_content_id",
                principalTable: "contents",
                principalColumn: "id");
        }
    }
}
