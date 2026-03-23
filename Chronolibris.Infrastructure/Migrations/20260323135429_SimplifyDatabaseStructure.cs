using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyDatabaseStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_contents_contents_parent_content_id",
                table: "contents");

            migrationBuilder.DropIndex(
                name: "ix_contents_parent_content_id",
                table: "contents");

            migrationBuilder.DropColumn(
                name: "parent_content_id",
                table: "contents");

            migrationBuilder.DropColumn(
                name: "order",
                table: "book_content");

            migrationBuilder.RenameColumn(
                name: "fts_configuration",
                table: "languages",
                newName: "code");

            migrationBuilder.AlterColumn<string>(
                name: "security_stamp",
                table: "users",
                type: "character varying(36)",
                unicode: false,
                maxLength: 36,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "users",
                type: "character varying(20)",
                unicode: false,
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "users",
                type: "character(256)",
                unicode: false,
                fixedLength: true,
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "concurrency_stamp",
                table: "users",
                type: "character(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "shelves",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "publishers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "persons",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "comment",
                table: "moderation_tasks",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "countries",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "note",
                table: "bookmarks",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "storage_url",
                table: "book_fragments",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_moderation_tasks_target_type_id",
                table: "moderation_tasks",
                column: "target_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_moderation_tasks_report_target_types_target_type_id",
                table: "moderation_tasks",
                column: "target_type_id",
                principalTable: "report_target_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_moderation_tasks_report_target_types_target_type_id",
                table: "moderation_tasks");

            migrationBuilder.DropIndex(
                name: "ix_moderation_tasks_target_type_id",
                table: "moderation_tasks");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "languages",
                newName: "fts_configuration");

            migrationBuilder.AlterColumn<string>(
                name: "security_stamp",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldUnicode: false,
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(256)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "concurrency_stamp",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "shelves",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "publishers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "persons",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "comment",
                table: "moderation_tasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "countries",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<long>(
                name: "parent_content_id",
                table: "contents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "note",
                table: "bookmarks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "storage_url",
                table: "book_fragments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048);

            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "book_content",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "book_content",
                keyColumns: new[] { "book_id", "content_id" },
                keyValues: new object[] { 1L, 1L },
                column: "order",
                value: 1);

            migrationBuilder.UpdateData(
                table: "book_content",
                keyColumns: new[] { "book_id", "content_id" },
                keyValues: new object[] { 2L, 2L },
                column: "order",
                value: 1);

            migrationBuilder.UpdateData(
                table: "contents",
                keyColumn: "id",
                keyValue: 1L,
                column: "parent_content_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "contents",
                keyColumn: "id",
                keyValue: 2L,
                column: "parent_content_id",
                value: null);

            migrationBuilder.CreateIndex(
                name: "ix_contents_parent_content_id",
                table: "contents",
                column: "parent_content_id");

            migrationBuilder.AddForeignKey(
                name: "fk_contents_contents_parent_content_id",
                table: "contents",
                column: "parent_content_id",
                principalTable: "contents",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
