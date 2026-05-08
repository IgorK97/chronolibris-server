using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBooksHasDataReq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "books",
            //    keyColumn: "id",
            //    keyValue: 1L);

            //migrationBuilder.DeleteData(
            //    table: "books",
            //    keyColumn: "id",
            //    keyValue: 2L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.InsertData(
            //    table: "books",
            //    columns: new[] { "id", "bbk", "country_id", "cover_path", "created_at", "description", "isbn", "is_available", "is_reviewable", "language_id", "publisher_id", "source", "title", "udk", "updated_at", "year" },
            //    values: new object[,]
            //    {
            //        { 1L, null, 1L, "BuddismHistory/BuddismJapanGrig/MainFile.png", new DateTime(2025, 11, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Монография является первой в отечественной литературе попыткой...", null, true, true, 2L, 2L, null, "Буддизм в Японии", null, null, 1993 },
            //        { 2L, null, 1L, "EconomicHistory/StructureBrodel/MainFile.png", new DateTime(2025, 11, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Это — второе крупное исследование Ф. Броделя...", null, true, true, 2L, 1L, null, "Структуры повседневности: возможное и невозможное", null, null, 1986 }
            //    });
        }
    }
}
