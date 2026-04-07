using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTagRelationTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tag_relation_type",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "description", "name" },
                values: new object[] { "Синонимия (теги обозначают одно и то же понятие)", "Синоним" });

            migrationBuilder.UpdateData(
                table: "tag_relation_type",
                keyColumn: "id",
                keyValue: 2L,
                column: "name",
                value: "Часть");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tag_relation_type",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "description", "name" },
                values: new object[] { "Синонимия", "synonym" });

            migrationBuilder.UpdateData(
                table: "tag_relation_type",
                keyColumn: "id",
                keyValue: 2L,
                column: "name",
                value: "part_of");
        }
    }
}
