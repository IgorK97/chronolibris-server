using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSelectionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_selections_selection_types_selection_type_id",
                table: "selections");

            migrationBuilder.DropTable(
                name: "selection_types");

            migrationBuilder.DropIndex(
                name: "ix_selections_selection_type_id",
                table: "selections");

            migrationBuilder.DropColumn(
                name: "selection_type_id",
                table: "selections");

            migrationBuilder.AddColumn<long>(
                name: "created_by",
                table: "selections",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 1L,
                column: "created_by",
                value: 1L);

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 2L,
                column: "created_by",
                value: 1L);

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 3L,
                column: "created_by",
                value: 1L);

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 4L,
                column: "created_by",
                value: 1L);

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 5L,
                column: "created_by",
                value: 1L);

            migrationBuilder.CreateIndex(
                name: "ix_selections_created_by",
                table: "selections",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "fk_selections_users_created_by",
                table: "selections",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_selections_users_created_by",
                table: "selections");

            migrationBuilder.DropIndex(
                name: "ix_selections_created_by",
                table: "selections");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "selections");

            migrationBuilder.AddColumn<int>(
                name: "selection_type_id",
                table: "selections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "selection_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_selection_types", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "selection_types",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Newest" },
                    { 2, "Popular" },
                    { 3, "Manual" }
                });

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 1L,
                column: "selection_type_id",
                value: 3);

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 2L,
                column: "selection_type_id",
                value: 3);

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 3L,
                column: "selection_type_id",
                value: 3);

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 4L,
                column: "selection_type_id",
                value: 1);

            migrationBuilder.UpdateData(
                table: "selections",
                keyColumn: "id",
                keyValue: 5L,
                column: "selection_type_id",
                value: 2);

            migrationBuilder.CreateIndex(
                name: "ix_selections_selection_type_id",
                table: "selections",
                column: "selection_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_selections_selection_types_selection_type_id",
                table: "selections",
                column: "selection_type_id",
                principalTable: "selection_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
