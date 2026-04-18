using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModerationTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_moderation_tasks_target_active_only",
                table: "moderation_tasks",
                columns: new[] { "target_id", "target_type_id" },
                unique: true,
                filter: "status_id = 2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_moderation_tasks_target_active_only",
                table: "moderation_tasks");
        }
    }
}
