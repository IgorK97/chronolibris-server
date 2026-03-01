using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitParticipationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "participations");

            migrationBuilder.CreateTable(
                name: "book_participations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    person_id = table.Column<long>(type: "bigint", nullable: false),
                    person_role_id = table.Column<long>(type: "bigint", nullable: false),
                    book_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book_participations", x => x.id);
                    table.ForeignKey(
                        name: "fk_book_participations_books_book_id",
                        column: x => x.book_id,
                        principalTable: "books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_book_participations_person_roles_person_role_id",
                        column: x => x.person_role_id,
                        principalTable: "person_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_book_participations_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "content_participations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    person_id = table.Column<long>(type: "bigint", nullable: false),
                    person_role_id = table.Column<long>(type: "bigint", nullable: false),
                    content_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_participations", x => x.id);
                    table.ForeignKey(
                        name: "fk_content_participations_contents_content_id",
                        column: x => x.content_id,
                        principalTable: "contents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_content_participations_person_roles_person_role_id",
                        column: x => x.person_role_id,
                        principalTable: "person_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_content_participations_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "content_participations",
                columns: new[] { "id", "content_id", "person_id", "person_role_id" },
                values: new object[,]
                {
                    { 1L, 1L, 1L, 1L },
                    { 2L, 2L, 2L, 1L }
                });

            migrationBuilder.CreateIndex(
                name: "ix_book_participations_book_id",
                table: "book_participations",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_participations_person_id",
                table: "book_participations",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_participations_person_role_id",
                table: "book_participations",
                column: "person_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_content_participations_content_id",
                table: "content_participations",
                column: "content_id");

            migrationBuilder.CreateIndex(
                name: "ix_content_participations_person_id",
                table: "content_participations",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_content_participations_person_role_id",
                table: "content_participations",
                column: "person_role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "book_participations");

            migrationBuilder.DropTable(
                name: "content_participations");

            migrationBuilder.CreateTable(
                name: "participations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    book_id = table.Column<long>(type: "bigint", nullable: true),
                    content_id = table.Column<long>(type: "bigint", nullable: true),
                    person_id = table.Column<long>(type: "bigint", nullable: false),
                    person_role_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_participations", x => x.id);
                    table.ForeignKey(
                        name: "fk_participations_books_book_id",
                        column: x => x.book_id,
                        principalTable: "books",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_participations_contents_content_id",
                        column: x => x.content_id,
                        principalTable: "contents",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_participations_person_roles_person_role_id",
                        column: x => x.person_role_id,
                        principalTable: "person_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_participations_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "participations",
                columns: new[] { "id", "book_id", "content_id", "person_id", "person_role_id" },
                values: new object[,]
                {
                    { 1L, null, 1L, 1L, 1L },
                    { 2L, null, 2L, 2L, 1L }
                });

            migrationBuilder.CreateIndex(
                name: "ix_participations_book_id",
                table: "participations",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "ix_participations_content_id",
                table: "participations",
                column: "content_id");

            migrationBuilder.CreateIndex(
                name: "ix_participations_person_id",
                table: "participations",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_participations_person_role_id",
                table: "participations",
                column: "person_role_id");
        }
    }
}
