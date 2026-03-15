using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkedContentsAndTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_books_tags_tag_id",
                table: "books");

            migrationBuilder.DropIndex(
                name: "ix_books_tag_id",
                table: "books");

            migrationBuilder.DropColumn(
                name: "tag_id",
                table: "books");

            migrationBuilder.CreateTable(
                name: "content_tags",
                columns: table => new
                {
                    contents_id = table.Column<long>(type: "bigint", nullable: false),
                    tags_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_tags", x => new { x.contents_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_content_tags_contents_contents_id",
                        column: x => x.contents_id,
                        principalTable: "contents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_content_tags_tags_tags_id",
                        column: x => x.tags_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_content_tags_tags_id",
                table: "content_tags",
                column: "tags_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "content_tags");

            migrationBuilder.AddColumn<long>(
                name: "tag_id",
                table: "books",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "id",
                keyValue: 1L,
                column: "tag_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "id",
                keyValue: 2L,
                column: "tag_id",
                value: null);

            migrationBuilder.CreateIndex(
                name: "ix_books_tag_id",
                table: "books",
                column: "tag_id");

            migrationBuilder.AddForeignKey(
                name: "fk_books_tags_tag_id",
                table: "books",
                column: "tag_id",
                principalTable: "tags",
                principalColumn: "id");
        }
    }
}
