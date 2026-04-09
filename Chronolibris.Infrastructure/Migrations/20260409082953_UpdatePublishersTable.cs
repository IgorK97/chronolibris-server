using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePublishersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "publishers",
                keyColumn: "id",
                keyValue: 1L,
                column: "created_at",
                value: new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "publishers",
                keyColumn: "id",
                keyValue: 2L,
                column: "created_at",
                value: new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "publishers",
                keyColumn: "id",
                keyValue: 1L,
                column: "created_at",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "publishers",
                keyColumn: "id",
                keyValue: 2L,
                column: "created_at",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}
