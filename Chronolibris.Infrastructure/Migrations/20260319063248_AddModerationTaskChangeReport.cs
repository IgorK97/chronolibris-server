using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModerationTaskChangeReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reports_report_statuses_status_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "moderated_at",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "moderated_by",
                table: "reports");

            migrationBuilder.RenameColumn(
                name: "status_id",
                table: "reports",
                newName: "moderation_task_id");

            migrationBuilder.RenameIndex(
                name: "ix_reports_status_id",
                table: "reports",
                newName: "ix_reports_moderation_task_id");

            migrationBuilder.CreateTable(
                name: "moderation_task",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    target_id = table.Column<long>(type: "bigint", nullable: false),
                    target_type_id = table.Column<long>(type: "bigint", nullable: false),
                    moderated_by = table.Column<long>(type: "bigint", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status_id = table.Column<long>(type: "bigint", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_moderation_task", x => x.id);
                    table.ForeignKey(
                        name: "fk_moderation_task_report_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "report_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_moderation_task_status_id",
                table: "moderation_task",
                column: "status_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reports_moderation_task_moderation_task_id",
                table: "reports",
                column: "moderation_task_id",
                principalTable: "moderation_task",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reports_moderation_task_moderation_task_id",
                table: "reports");

            migrationBuilder.DropTable(
                name: "moderation_task");

            migrationBuilder.RenameColumn(
                name: "moderation_task_id",
                table: "reports",
                newName: "status_id");

            migrationBuilder.RenameIndex(
                name: "ix_reports_moderation_task_id",
                table: "reports",
                newName: "ix_reports_status_id");

            migrationBuilder.AddColumn<DateTime>(
                name: "moderated_at",
                table: "reports",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "moderated_by",
                table: "reports",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_reports_report_statuses_status_id",
                table: "reports",
                column: "status_id",
                principalTable: "report_statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
