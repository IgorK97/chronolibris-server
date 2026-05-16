using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClarifyBehaviours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_book_content_books_book_id",
                table: "book_content");

            migrationBuilder.DropForeignKey(
                name: "fk_book_content_contents_content_id",
                table: "book_content");

            migrationBuilder.DropForeignKey(
                name: "fk_book_participations_persons_person_id",
                table: "book_participations");

            migrationBuilder.DropForeignKey(
                name: "fk_books_countries_country_id",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "fk_books_languages_language_id",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "fk_books_publishers_publisher_id",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "fk_content_participations_persons_person_id",
                table: "content_participations");

            migrationBuilder.DropForeignKey(
                name: "fk_content_tags_tags_tags_id",
                table: "content_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_contents_countries_country_id",
                table: "contents");

            migrationBuilder.DropForeignKey(
                name: "fk_contents_languages_language_id",
                table: "contents");

            migrationBuilder.AddForeignKey(
                name: "fk_book_content_books_book_id",
                table: "book_content",
                column: "book_id",
                principalTable: "books",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_book_content_contents_content_id",
                table: "book_content",
                column: "content_id",
                principalTable: "contents",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_book_participations_persons_person_id",
                table: "book_participations",
                column: "person_id",
                principalTable: "persons",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_books_countries_country_id",
                table: "books",
                column: "country_id",
                principalTable: "countries",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_books_languages_language_id",
                table: "books",
                column: "language_id",
                principalTable: "languages",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_books_publishers_publisher_id",
                table: "books",
                column: "publisher_id",
                principalTable: "publishers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_content_participations_persons_person_id",
                table: "content_participations",
                column: "person_id",
                principalTable: "persons",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_content_tags_tags_tags_id",
                table: "content_tags",
                column: "tags_id",
                principalTable: "tags",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_contents_countries_country_id",
                table: "contents",
                column: "country_id",
                principalTable: "countries",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_contents_languages_language_id",
                table: "contents",
                column: "language_id",
                principalTable: "languages",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_book_content_books_book_id",
                table: "book_content");

            migrationBuilder.DropForeignKey(
                name: "fk_book_content_contents_content_id",
                table: "book_content");

            migrationBuilder.DropForeignKey(
                name: "fk_book_participations_persons_person_id",
                table: "book_participations");

            migrationBuilder.DropForeignKey(
                name: "fk_books_countries_country_id",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "fk_books_languages_language_id",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "fk_books_publishers_publisher_id",
                table: "books");

            migrationBuilder.DropForeignKey(
                name: "fk_content_participations_persons_person_id",
                table: "content_participations");

            migrationBuilder.DropForeignKey(
                name: "fk_content_tags_tags_tags_id",
                table: "content_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_contents_countries_country_id",
                table: "contents");

            migrationBuilder.DropForeignKey(
                name: "fk_contents_languages_language_id",
                table: "contents");

            migrationBuilder.AddForeignKey(
                name: "fk_book_content_books_book_id",
                table: "book_content",
                column: "book_id",
                principalTable: "books",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_book_content_contents_content_id",
                table: "book_content",
                column: "content_id",
                principalTable: "contents",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_book_participations_persons_person_id",
                table: "book_participations",
                column: "person_id",
                principalTable: "persons",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_books_countries_country_id",
                table: "books",
                column: "country_id",
                principalTable: "countries",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_books_languages_language_id",
                table: "books",
                column: "language_id",
                principalTable: "languages",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_books_publishers_publisher_id",
                table: "books",
                column: "publisher_id",
                principalTable: "publishers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_content_participations_persons_person_id",
                table: "content_participations",
                column: "person_id",
                principalTable: "persons",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_content_tags_tags_tags_id",
                table: "content_tags",
                column: "tags_id",
                principalTable: "tags",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_contents_countries_country_id",
                table: "contents",
                column: "country_id",
                principalTable: "countries",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_contents_languages_language_id",
                table: "contents",
                column: "language_id",
                principalTable: "languages",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
