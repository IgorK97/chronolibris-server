using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModerationTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_moderation_tasks_report_reasons_reason_type_id",
                table: "moderation_tasks");

            migrationBuilder.DropIndex(
                name: "ix_moderation_tasks_reason_type_id",
                table: "moderation_tasks");

            migrationBuilder.DropIndex(
                name: "ix_moderation_tasks_target_active_only",
                table: "moderation_tasks");

            migrationBuilder.DropColumn(
                name: "check_number",
                table: "moderation_tasks");

            migrationBuilder.DropColumn(
                name: "reason_type_id",
                table: "moderation_tasks");

            migrationBuilder.AlterColumn<string>(
                name: "comment",
                table: "moderation_tasks",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<long>(
                name: "report_reason_type_id",
                table: "moderation_tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_moderation_tasks_one_active_only",
                table: "moderation_tasks",
                columns: new[] { "target_id", "target_type_id" },
                unique: true,
                filter: "status_id = 2");

            migrationBuilder.CreateIndex(
                name: "ix_moderation_tasks_report_reason_type_id",
                table: "moderation_tasks",
                column: "report_reason_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_moderation_tasks_report_reasons_report_reason_type_id",
                table: "moderation_tasks",
                column: "report_reason_type_id",
                principalTable: "report_reasons",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_moderation_tasks_report_reasons_report_reason_type_id",
                table: "moderation_tasks");

            migrationBuilder.DropIndex(
                name: "ix_moderation_tasks_one_active_only",
                table: "moderation_tasks");

            migrationBuilder.DropIndex(
                name: "ix_moderation_tasks_report_reason_type_id",
                table: "moderation_tasks");

            migrationBuilder.DropColumn(
                name: "report_reason_type_id",
                table: "moderation_tasks");

            migrationBuilder.AlterColumn<string>(
                name: "comment",
                table: "moderation_tasks",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            migrationBuilder.AddColumn<int>(
                name: "check_number",
                table: "moderation_tasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "reason_type_id",
                table: "moderation_tasks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "ix_moderation_tasks_reason_type_id",
                table: "moderation_tasks",
                column: "reason_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_moderation_tasks_target_active_only",
                table: "moderation_tasks",
                columns: new[] { "target_id", "target_type_id", "reason_type_id" },
                unique: true,
                filter: "status_id = 2");

            migrationBuilder.AddForeignKey(
                name: "fk_moderation_tasks_report_reasons_reason_type_id",
                table: "moderation_tasks",
                column: "reason_type_id",
                principalTable: "report_reasons",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
