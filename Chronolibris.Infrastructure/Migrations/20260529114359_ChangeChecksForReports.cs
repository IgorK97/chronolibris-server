using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeChecksForReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_report_target",
                table: "reports");

            migrationBuilder.DropCheckConstraint(
                name: "ck_moderation_task_target",
                table: "moderation_tasks");

            migrationBuilder.AddCheckConstraint(
                name: "ck_report_target",
                table: "reports",
                sql: "((book_id IS NOT NULL)::int + (review_id IS NOT NULL)::int + (comment_id IS NOT NULL)::int) = 1");

            migrationBuilder.AddCheckConstraint(
                name: "ck_moderation_task_target",
                table: "moderation_tasks",
                sql: "((book_id IS NOT NULL)::int + (review_id IS NOT NULL)::int + (comment_id IS NOT NULL)::int) = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_report_target",
                table: "reports");

            migrationBuilder.DropCheckConstraint(
                name: "ck_moderation_task_target",
                table: "moderation_tasks");

            migrationBuilder.AddCheckConstraint(
                name: "ck_report_target",
                table: "reports",
                sql: "book_id IS NOT NULL OR review_id IS NOT NULL OR comment_id IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "ck_moderation_task_target",
                table: "moderation_tasks",
                sql: "book_id IS NOT NULL OR review_id IS NOT NULL OR comment_id IS NOT NULL");
        }
    }
}
