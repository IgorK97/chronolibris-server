using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLengthChecksBooksContents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_contents_description_min_length",
                table: "contents");

            migrationBuilder.DropCheckConstraint(
                name: "CK_books_description_min_length",
                table: "books");

            migrationBuilder.AddCheckConstraint(
                name: "CK_contents_description_min_length",
                table: "contents",
                sql: "LENGTH(TRIM(description))>=120");

            migrationBuilder.AddCheckConstraint(
                name: "CK_books_description_min_length",
                table: "books",
                sql: "LENGTH(TRIM(description)) >= 120");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_contents_description_min_length",
                table: "contents");

            migrationBuilder.DropCheckConstraint(
                name: "CK_books_description_min_length",
                table: "books");

            migrationBuilder.AddCheckConstraint(
                name: "CK_contents_description_min_length",
                table: "contents",
                sql: "LENGTH(description)>=120");

            migrationBuilder.AddCheckConstraint(
                name: "CK_books_description_min_length",
                table: "books",
                sql: "LENGTH(description) >= 120");
        }
    }
}
