using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTagTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<PersonRoleKind>(
                name: "kind",
                table: "person_roles",
                type: "person_role_kind",
                nullable: false,
                defaultValue: PersonRoleKind.Both,
                oldClrType: typeof(PersonRoleKind),
                oldType: "person_role_kind",
                oldDefaultValueSql: "'both'::person_role_kind");

            migrationBuilder.AlterColumn<ContentNature>(
                name: "nature",
                table: "content_type",
                type: "content_nature_enum",
                nullable: false,
                oldClrType: typeof(ContentNature),
                oldType: "content_nature_enum",
                oldDefaultValueSql: "'unknown'::content_nature_enum");

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 2L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 3L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 4L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 5L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 6L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 7L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 8L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 9L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 10L,
                column: "kind",
                value: PersonRoleKind.Both);

            migrationBuilder.UpdateData(
                table: "tag_types",
                keyColumn: "id",
                keyValue: 1L,
                column: "name",
                value: "Время");

            migrationBuilder.UpdateData(
                table: "tag_types",
                keyColumn: "id",
                keyValue: 2L,
                column: "name",
                value: "Место");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<PersonRoleKind>(
                name: "kind",
                table: "person_roles",
                type: "person_role_kind",
                nullable: false,
                defaultValueSql: "'both'::person_role_kind",
                oldClrType: typeof(PersonRoleKind),
                oldType: "person_role_kind",
                oldDefaultValue: PersonRoleKind.Both);

            migrationBuilder.AlterColumn<ContentNature>(
                name: "nature",
                table: "content_type",
                type: "content_nature_enum",
                nullable: false,
                defaultValueSql: "'unknown'::content_nature_enum",
                oldClrType: typeof(ContentNature),
                oldType: "content_nature_enum");

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "kind",
                value: 0);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 2L,
                column: "kind",
                value: 0);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 3L,
                column: "kind",
                value: 0);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 4L,
                column: "kind",
                value: 0);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 5L,
                column: "kind",
                value: 0);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 6L,
                column: "kind",
                value: 0);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 7L,
                column: "kind",
                value: 0);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 8L,
                column: "kind",
                value: 0);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 9L,
                column: "kind",
                value: 0);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 10L,
                column: "kind",
                value: 0);

            migrationBuilder.UpdateData(
                table: "tag_types",
                keyColumn: "id",
                keyValue: 1L,
                column: "name",
                value: "Место");

            migrationBuilder.UpdateData(
                table: "tag_types",
                keyColumn: "id",
                keyValue: 2L,
                column: "name",
                value: "Время");
        }
    }
}
