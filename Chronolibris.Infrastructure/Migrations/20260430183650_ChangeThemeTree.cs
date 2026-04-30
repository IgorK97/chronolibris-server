using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeThemeTree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "themes",
                keyColumn: "id",
                keyValue: 27L);

            migrationBuilder.DeleteData(
                table: "themes",
                keyColumn: "id",
                keyValue: 28L);

            migrationBuilder.DeleteData(
                table: "themes",
                keyColumn: "id",
                keyValue: 29L);

            migrationBuilder.DeleteData(
                table: "themes",
                keyColumn: "id",
                keyValue: 30L);

            migrationBuilder.DeleteData(
                table: "themes",
                keyColumn: "id",
                keyValue: 31L);

            migrationBuilder.DeleteData(
                table: "themes",
                keyColumn: "id",
                keyValue: 32L);

            migrationBuilder.DeleteData(
                table: "themes",
                keyColumn: "id",
                keyValue: 33L);

            migrationBuilder.DeleteData(
                table: "themes",
                keyColumn: "id",
                keyValue: 34L);

            migrationBuilder.DeleteData(
                table: "themes",
                keyColumn: "id",
                keyValue: 35L);

            migrationBuilder.DeleteData(
                table: "themes",
                keyColumn: "id",
                keyValue: 36L);

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 1L,
                column: "name",
                value: "Политическая история");

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 2L,
                column: "name",
                value: "Военная история");

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 4L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Китая", 12L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 5L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Японии", 12L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 6L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Искусство", null });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 7L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Религия", null });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 8L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Всеобщая и мировая история", 1L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 9L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Региональная и национальная история", 1L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 10L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Европы", 9L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 11L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История России", 9L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 12L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Азии", 9L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 13L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Африки", 9L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 14L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Америки", 9L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 15L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Австралазии и Тихоокеании", 9L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 16L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История других земель", 9L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 17L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История: отдельные темы и события", 1L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 18L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Западная философия", 3L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 19L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Античная философия", 18L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 20L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Философия Средних веков и эпохи Ренессанса", 18L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 21L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Философия эпохи Нового времени", 18L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 22L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Современная философия", 18L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 23L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Восточная философия", 3L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 24L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Исламская и арабская философия", 3L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 25L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Социальная и политическая философия", 3L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 26L,
                column: "name",
                value: "Русская философия");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 1L,
                column: "name",
                value: "История");

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 2L,
                column: "name",
                value: "Археология");

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 4L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Религия", null });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 5L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Всеобщая и мировая история", 1L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 6L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Региональная и национальная история", 1L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 7L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Европы", 6L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 8L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История России", 7L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 9L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Азии", 6L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 10L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Африки", 6L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 11L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Америки", 6L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 12L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Австралазии и Тихоокеании", 6L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 13L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История других земель", 6L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 14L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История от древнейших времен до сегодняшнего дня", 1L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 15L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Древнего мира", 14L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 16L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Средневековья", 14L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 17L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История Нового времени", 14L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 18L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История 20 века", 14L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 19L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История 21 века", 14L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 20L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "История: отдельные темы и события", 1L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 21L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Военная история", 1L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 22L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Гражданская война в России", 21L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 23L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Первая мировая война", 21L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 24L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Вторая мировая война", 21L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 25L,
                columns: new[] { "name", "parent_theme_id" },
                values: new object[] { "Крымская война", 21L });

            migrationBuilder.UpdateData(
                table: "themes",
                keyColumn: "id",
                keyValue: 26L,
                column: "name",
                value: "История западной философии");

            migrationBuilder.InsertData(
                table: "themes",
                columns: new[] { "id", "name", "parent_theme_id" },
                values: new object[,]
                {
                    { 27L, "Античная философия", 26L },
                    { 28L, "Философия Средних веков и эпохи Ренессанса", 26L },
                    { 29L, "Философия эпохи Нового времени", 26L },
                    { 30L, "Современная философия", 26L },
                    { 31L, "Восточная философия", 3L },
                    { 32L, "Исламская и арабская философия", 3L },
                    { 33L, "Социальная и политическая философия", 3L },
                    { 34L, "Христианство", 4L },
                    { 35L, "Буддизм", 4L },
                    { 36L, "Ислам", 4L }
                });
        }
    }
}
