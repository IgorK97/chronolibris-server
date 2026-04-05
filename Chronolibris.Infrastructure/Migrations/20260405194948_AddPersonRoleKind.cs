using System;
using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonRoleKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "formats",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "formats",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "formats",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "selections",
                keyColumn: "id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "selections",
                keyColumn: "id",
                keyValue: 5L);

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:person_role_kind", "content,book,both");

            migrationBuilder.AddColumn<PersonRoleKind>(
                name: "kind",
                table: "person_roles",
                type: "person_role_kind",
                nullable: false,
                defaultValueSql: "'both'::person_role_kind");

            //migrationBuilder.UpdateData(
            //    table: "person_roles",
            //    keyColumn: "id",
            //    keyValue: 1L,
            //    columns: new string[0],
            //    values: new object[0]);

            //migrationBuilder.UpdateData(
            //    table: "person_roles",
            //    keyColumn: "id",
            //    keyValue: 2L,
            //    columns: new string[0],
            //    values: new object[0]);

            //migrationBuilder.UpdateData(
            //    table: "person_roles",
            //    keyColumn: "id",
            //    keyValue: 3L,
            //    columns: new string[0],
            //    values: new object[0]);

            //migrationBuilder.UpdateData(
            //    table: "person_roles",
            //    keyColumn: "id",
            //    keyValue: 4L,
            //    columns: new string[0],
            //    values: new object[0]);

            //migrationBuilder.UpdateData(
            //    table: "person_roles",
            //    keyColumn: "id",
            //    keyValue: 5L,
            //    columns: new string[0],
            //    values: new object[0]);

            //migrationBuilder.UpdateData(
            //    table: "person_roles",
            //    keyColumn: "id",
            //    keyValue: 6L,
            //    columns: new string[0],
            //    values: new object[0]);

            //migrationBuilder.UpdateData(
            //    table: "person_roles",
            //    keyColumn: "id",
            //    keyValue: 7L,
            //    columns: new string[0],
            //    values: new object[0]);

            //migrationBuilder.UpdateData(
            //    table: "person_roles",
            //    keyColumn: "id",
            //    keyValue: 8L,
            //    columns: new string[0],
            //    values: new object[0]);

            //migrationBuilder.UpdateData(
            //    table: "person_roles",
            //    keyColumn: "id",
            //    keyValue: 9L,
            //    columns: new string[0],
            //    values: new object[0]);

            //migrationBuilder.UpdateData(
            //    table: "person_roles",
            //    keyColumn: "id",
            //    keyValue: 10L,
            //    columns: new string[0],
            //    values: new object[0]);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 11L,
                column: "name",
                value: "Комментатор");

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 12L,
                column: "name",
                value: "Адресат");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "kind",
                table: "person_roles");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:person_role_kind", "content,book,both");

            migrationBuilder.InsertData(
                table: "formats",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 2, "epub" },
                    { 3, "pdf" },
                    { 4, "txt" }
                });

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 11L,
                column: "name",
                value: "Оцифровщик");

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 12L,
                column: "name",
                value: "Автор предисловия");

            migrationBuilder.InsertData(
                table: "person_roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 13L, "Автор послесловия" },
                    { 14L, "Комментатор" },
                    { 15L, "Дизайнер" }
                });

            migrationBuilder.InsertData(
                table: "selections",
                columns: new[] { "id", "created_at", "description", "is_active", "name", "updated_at", "user_id" },
                values: new object[,]
                {
                    { 4L, new DateTime(2025, 11, 20, 0, 0, 0, 0, DateTimeKind.Utc), "", true, "Новое", null, 1L },
                    { 5L, new DateTime(2025, 11, 20, 0, 0, 0, 0, DateTimeKind.Utc), "", true, "Часто читают", null, 1L }
                });
        }
    }
}
