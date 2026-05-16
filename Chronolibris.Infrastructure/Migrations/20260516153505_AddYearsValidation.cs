using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYearsValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION check_book_content_chronology()
            RETURNS TRIGGER AS $$
            DECLARE
               invalid_exists BOOLEAN;
            BEGIN
               SELECT EXISTS (
                  SELECT 1
                  FROM book_content bc
                  JOIN books b ON b.id = bc.book_id
                  JOIN contents c ON c.id = bc.content_id
                  WHERE b.year IS NOT NULL
                     AND (c.year_from IS NOT NULL OR c.year_to IS NOT NULL)
                     AND b.year < COALESCE(c.year_from, c.year_to)
               ) INTO invalid_exists;

               IF invalid_exists THEN
                  RAISE EXCEPTION 'Хронологическая ошибка: год обнародования книги не может быть раньше начала создания входящего в него контента' USING ERRCODE='45001';
               END IF;

               RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(@"
            CREATE TRIGGER trg_bc_chronology
            AFTER INSERT OR UPDATE ON book_content
            FOR EACH STATEMENT EXECUTE FUNCTION check_book_content_chronology();");

            migrationBuilder.Sql(@"
            CREATE TRIGGER trg_books_chronology
            AFTER UPDATE OF year ON books
            FOR EACH STATEMENT EXECUTE FUNCTION check_book_content_chronology();");

            migrationBuilder.Sql(@"
            CREATE TRIGGER trg_contents_chronology
            AFTER UPDATE OF year_from, year_to ON contents
            FOR EACH STATEMENT EXECUTE FUNCTION check_book_content_chronology();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_bc_chronology ON book_content;");

            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_books_chronology ON books;");

            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_contents_chronology ON contents;");

            migrationBuilder.Sql(@"
            DROP FUNCTION IF EXISTS check_book_content_chronology();");
        }
    }
}
