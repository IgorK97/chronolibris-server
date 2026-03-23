using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteSeriesTableDeleteIsFragmentField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_books_series_series_id",
                table: "books");

            migrationBuilder.DropTable(
                name: "series");

            migrationBuilder.DropIndex(
                name: "ix_books_series_id",
                table: "books");

            migrationBuilder.DropColumn(
                name: "is_fragment",
                table: "books");

            migrationBuilder.DropColumn(
                name: "series_id",
                table: "books");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_fragment",
                table: "books",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "series_id",
                table: "books",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "series",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    publisher_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_series", x => x.id);
                    table.ForeignKey(
                        name: "fk_series_publishers_publisher_id",
                        column: x => x.publisher_id,
                        principalTable: "publishers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "is_fragment", "series_id" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "id",
                keyValue: 2L,
                columns: new[] { "is_fragment", "series_id" },
                values: new object[] { false, null });

            migrationBuilder.CreateIndex(
                name: "ix_books_series_id",
                table: "books",
                column: "series_id");

            migrationBuilder.CreateIndex(
                name: "ix_series_publisher_id",
                table: "series",
                column: "publisher_id");

            migrationBuilder.AddForeignKey(
                name: "fk_books_series_series_id",
                table: "books",
                column: "series_id",
                principalTable: "series",
                principalColumn: "id");
        }
    }
}
