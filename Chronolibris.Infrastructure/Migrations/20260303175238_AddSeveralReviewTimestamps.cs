using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeveralReviewTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "reviews",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "moderated_at",
                table: "reviews",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "review_status_id",
                table: "reviews",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "reviews",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "review_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_review_status", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "review_status",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1L, "На проверке" },
                    { 2L, "Опубликован" },
                    { 3L, "Отклонен" },
                    { 4L, "Удален" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_reviews_review_status_id",
                table: "reviews",
                column: "review_status_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reviews_review_status_review_status_id",
                table: "reviews",
                column: "review_status_id",
                principalTable: "review_status",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reviews_review_status_review_status_id",
                table: "reviews");

            migrationBuilder.DropTable(
                name: "review_status");

            migrationBuilder.DropIndex(
                name: "ix_reviews_review_status_id",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "moderated_at",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "review_status_id",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "reviews");
        }
    }
}
