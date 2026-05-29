using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorReports2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_moderation_tasks_report_target_types_target_type_id",
                table: "moderation_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_reports_report_target_types_target_type_id",
                table: "reports");

            migrationBuilder.DropTable(
                name: "report_target_types");

            migrationBuilder.DropIndex(
                name: "ix_reports_target_type_id",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "ix_moderation_tasks_one_active_only",
                table: "moderation_tasks");

            migrationBuilder.DropIndex(
                name: "ix_moderation_tasks_target_type_id",
                table: "moderation_tasks");

            migrationBuilder.DropColumn(
                name: "target_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "target_type_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "target_id",
                table: "moderation_tasks");

            migrationBuilder.DropColumn(
                name: "target_type_id",
                table: "moderation_tasks");

            migrationBuilder.RenameColumn(
                name: "comment",
                table: "moderation_tasks",
                newName: "comment_text");

            migrationBuilder.AddColumn<long>(
                name: "book_id",
                table: "reports",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "comment_id",
                table: "reports",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "review_id",
                table: "reports",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "book_id",
                table: "moderation_tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "comment_id",
                table: "moderation_tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "review_id",
                table: "moderation_tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_reports_book_id",
                table: "reports",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "ix_reports_comment_id",
                table: "reports",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_reports_review_id",
                table: "reports",
                column: "review_id");

            migrationBuilder.AddCheckConstraint(
                name: "ck_report_target",
                table: "reports",
                sql: "book_id IS NOT NULL OR review_id IS NOT NULL OR comment_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_moderation_tasks_one_book_only",
                table: "moderation_tasks",
                column: "book_id",
                unique: true,
                filter: "status_id = 2");

            migrationBuilder.CreateIndex(
                name: "ix_moderation_tasks_one_comment_only",
                table: "moderation_tasks",
                column: "comment_id",
                unique: true,
                filter: "status_id = 2");

            migrationBuilder.CreateIndex(
                name: "ix_moderation_tasks_one_review_only",
                table: "moderation_tasks",
                column: "review_id",
                unique: true,
                filter: "status_id = 2");

            migrationBuilder.AddCheckConstraint(
                name: "ck_moderation_task_target",
                table: "moderation_tasks",
                sql: "book_id IS NOT NULL OR review_id IS NOT NULL OR comment_id IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "fk_moderation_tasks_books_book_id",
                table: "moderation_tasks",
                column: "book_id",
                principalTable: "books",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderation_tasks_comments_comment_id",
                table: "moderation_tasks",
                column: "comment_id",
                principalTable: "comments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_moderation_tasks_reviews_review_id",
                table: "moderation_tasks",
                column: "review_id",
                principalTable: "reviews",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_reports_books_book_id",
                table: "reports",
                column: "book_id",
                principalTable: "books",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_reports_comments_comment_id",
                table: "reports",
                column: "comment_id",
                principalTable: "comments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_reports_reviews_review_id",
                table: "reports",
                column: "review_id",
                principalTable: "reviews",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_moderation_tasks_books_book_id",
                table: "moderation_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_moderation_tasks_comments_comment_id",
                table: "moderation_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_moderation_tasks_reviews_review_id",
                table: "moderation_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_reports_books_book_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "fk_reports_comments_comment_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "fk_reports_reviews_review_id",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "ix_reports_book_id",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "ix_reports_comment_id",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "ix_reports_review_id",
                table: "reports");

            migrationBuilder.DropCheckConstraint(
                name: "ck_report_target",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "ix_moderation_tasks_one_book_only",
                table: "moderation_tasks");

            migrationBuilder.DropIndex(
                name: "ix_moderation_tasks_one_comment_only",
                table: "moderation_tasks");

            migrationBuilder.DropIndex(
                name: "ix_moderation_tasks_one_review_only",
                table: "moderation_tasks");

            migrationBuilder.DropCheckConstraint(
                name: "ck_moderation_task_target",
                table: "moderation_tasks");

            migrationBuilder.DropColumn(
                name: "book_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "comment_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "review_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "book_id",
                table: "moderation_tasks");

            migrationBuilder.DropColumn(
                name: "comment_id",
                table: "moderation_tasks");

            migrationBuilder.DropColumn(
                name: "review_id",
                table: "moderation_tasks");

            migrationBuilder.RenameColumn(
                name: "comment_text",
                table: "moderation_tasks",
                newName: "comment");

            migrationBuilder.AddColumn<long>(
                name: "target_id",
                table: "reports",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "target_type_id",
                table: "reports",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "target_id",
                table: "moderation_tasks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "target_type_id",
                table: "moderation_tasks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "report_target_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_report_target_types", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "report_target_types",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1L, "Книги" },
                    { 2L, "Отзывы" },
                    { 3L, "Комментарии" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_reports_target_type_id",
                table: "reports",
                column: "target_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_moderation_tasks_one_active_only",
                table: "moderation_tasks",
                columns: new[] { "target_id", "target_type_id" },
                unique: true,
                filter: "status_id = 2");

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

            migrationBuilder.AddForeignKey(
                name: "fk_reports_report_target_types_target_type_id",
                table: "reports",
                column: "target_type_id",
                principalTable: "report_target_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
