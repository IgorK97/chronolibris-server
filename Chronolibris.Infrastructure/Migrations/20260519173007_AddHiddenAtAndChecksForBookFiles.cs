using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHiddenAtAndChecksForBookFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "hidden_at",
                table: "book_files",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_book_files_archive_rule",
                table: "book_files",
                sql: "(status_id = 6 AND is_readable = true AND hidden_at IS NOT NULL) OR (status_id != 6 AND hidden_at IS NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_book_files_archive_rule",
                table: "book_files");

            migrationBuilder.DropColumn(
                name: "hidden_at",
                table: "book_files");
        }
    }
}
