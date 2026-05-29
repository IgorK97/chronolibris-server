using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeFormatPK2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_book_files_formats_format_id",
                table: "book_files");

            migrationBuilder.DeleteData(
                table: "formats",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "formats",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "formats",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "format_id",
                table: "book_files",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                table: "formats",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1L, "fb2" },
                    { 2L, "epub" }
                });

            migrationBuilder.AddForeignKey(
                name: "fk_book_files_formats_format_id",
                table: "book_files",
                column: "format_id",
                principalTable: "formats",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_book_files_formats_format_id",
                table: "book_files");

            migrationBuilder.DeleteData(
                table: "formats",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "formats",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "formats",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "format_id",
                table: "book_files",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.InsertData(
                table: "formats",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "fb2" },
                    { 2, "epub" }
                });

            migrationBuilder.AddForeignKey(
                name: "fk_book_files_formats_format_id",
                table: "book_files",
                column: "format_id",
                principalTable: "formats",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
