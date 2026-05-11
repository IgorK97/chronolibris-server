using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDefaultData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "languages",
            //    keyColumn: "id",
            //    keyValue: 5L);

            migrationBuilder.UpdateData(
                table: "publishers",
                keyColumn: "id",
                keyValue: 1L,
                column: "description",
                value: "«Прогре́сс» — советское государственное и российское частное издательство. Основано в 1964 году на базе гуманитарных редакций «Издательства иностранной литературы» и «Издательства литературы на иностранных языках». Специализировалось на выпуске переводной гуманитарной и художественной литературы.");

            migrationBuilder.UpdateData(
                table: "publishers",
                keyColumn: "id",
                keyValue: 2L,
                column: "description",
                value: "«Восто́чная литерату́ра» РАН — издательская фирма, специализирующаяся на различных отраслях востоковедения и африканистики.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.InsertData(
            //    table: "languages",
            //    columns: new[] { "id", "name" },
            //    values: new object[] { 5L, "Русский (дореформенная орфография)" });

            migrationBuilder.UpdateData(
                table: "publishers",
                keyColumn: "id",
                keyValue: 1L,
                column: "description",
                value: "");

            migrationBuilder.UpdateData(
                table: "publishers",
                keyColumn: "id",
                keyValue: 2L,
                column: "description",
                value: "");
        }
    }
}
