using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixReportReasonType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "fk_moderation_tasks_report_reasons_report_reason_type_id",
            //    table: "moderation_tasks");

            //migrationBuilder.DropIndex(
            //    name: "ix_moderation_tasks_report_reason_type_id",
            //    table: "moderation_tasks");

            //migrationBuilder.DropColumn(
            //    name: "report_reason_type_id",
            //    table: "moderation_tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<long>(
            //    name: "report_reason_type_id",
            //    table: "moderation_tasks",
            //    type: "bigint",
            //    nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "ix_moderation_tasks_report_reason_type_id",
            //    table: "moderation_tasks",
            //    column: "report_reason_type_id");

            //migrationBuilder.AddForeignKey(
            //    name: "fk_moderation_tasks_report_reasons_report_reason_type_id",
            //    table: "moderation_tasks",
            //    column: "report_reason_type_id",
            //    principalTable: "report_reasons",
            //    principalColumn: "id");
        }
    }
}
