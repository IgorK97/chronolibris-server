using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookFileStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "processed_at",
                table: "book_files",
                newName: "completed_at");

            migrationBuilder.AddColumn<long>(
                name: "book_file_status_id",
                table: "book_files",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "book_file_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book_file_statuses", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "book_file_statuses",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1L, "В ожидании загрузки" },
                    { 2L, "Файл загружен, не обработан" },
                    { 3L, "Обработка" },
                    { 4L, "Готов" },
                    { 5L, "Ошибка" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_book_files_book_file_status_id",
                table: "book_files",
                column: "book_file_status_id");

            migrationBuilder.AddForeignKey(
                name: "fk_book_files_book_file_statuses_book_file_status_id",
                table: "book_files",
                column: "book_file_status_id",
                principalTable: "book_file_statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_book_files_book_file_statuses_book_file_status_id",
                table: "book_files");

            migrationBuilder.DropTable(
                name: "book_file_statuses");

            migrationBuilder.DropIndex(
                name: "ix_book_files_book_file_status_id",
                table: "book_files");

            migrationBuilder.DropColumn(
                name: "book_file_status_id",
                table: "book_files");

            migrationBuilder.RenameColumn(
                name: "completed_at",
                table: "book_files",
                newName: "processed_at");
        }
    }
}
