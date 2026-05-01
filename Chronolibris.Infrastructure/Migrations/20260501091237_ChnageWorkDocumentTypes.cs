using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChnageWorkDocumentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 16L,
                column: "nature",
                value: ContentNature.Document);

            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 19L,
                column: "nature",
                value: ContentNature.Work);

            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 20L,
                column: "nature",
                value: ContentNature.Work);

            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 21L,
                column: "nature",
                value: ContentNature.Work);

            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 23L,
                column: "nature",
                value: ContentNature.Work);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 16L,
                column: "nature",
                value: ContentNature.Work);

            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 19L,
                column: "nature",
                value: ContentNature.Analysis);

            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 20L,
                column: "nature",
                value: ContentNature.Analysis);

            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 21L,
                column: "nature",
                value: ContentNature.Analysis);

            migrationBuilder.UpdateData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 23L,
                column: "nature",
                value: ContentNature.Analysis);
        }
    }
}
