using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSearchLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_books_title_change ON books;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS update_book_self();");

            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_contents_search ON contents;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS update_books_by_content();");

            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_book_content_search ON book_content;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS update_book_data_vector();");

            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_books_search_data_trgm\";");

            migrationBuilder.DropColumn(
                name: "search_data",
                table: "books");

            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS IX_books_title_trgm ON ""books"" 
                USING gin (""title"" gin_trgm_ops);
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS IX_contents_title_trgm ON ""contents"" 
                USING gin (""title"" gin_trgm_ops);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "search_data",
                table: "books",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "id",
                keyValue: 1L,
                column: "search_data",
                value: null);

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "id",
                keyValue: 2L,
                column: "search_data",
                value: null);

            migrationBuilder.Sql(@"CREATE EXTENSION IF NOT EXISTS pg_trgm;");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_books_search_data_trgm"" 
                                   ON books USING GIN (search_data gin_trgm_ops);");

            migrationBuilder.Sql(@"UPDATE books b
                                   SET search_data = b.title || ' ' || coalesce((
                                   SELECT string_agg(c.title, ' ')
                                   FROM contents c
                                   JOIN book_content bc ON bc.content_id = c.id
                                   WHERE bc.book_id = b.id
                                   ), '');");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION update_book_data_vector()
                                    RETURNS TRIGGER AS $$
                                    BEGIN
                                      -- Общая функция пересчёта для одной книги
                                      -- Обновляем OLD книгу (при UPDATE и DELETE)
                                      IF (TG_OP = 'UPDATE' OR TG_OP = 'DELETE') THEN
                                        UPDATE books b
                                        SET search_data = b.title || ' ' || coalesce((
                                          SELECT string_agg(c.title, ' ')
                                          FROM contents c
                                          JOIN book_content bc ON bc.content_id = c.id
                                          WHERE bc.book_id = b.id
                                        ), '')
                                        WHERE b.id = OLD.book_id;
                                      END IF;

                                      -- Обновляем NEW книгу (при UPDATE и INSERT)
                                      IF (TG_OP = 'UPDATE' OR TG_OP = 'INSERT') THEN
                                        UPDATE books b
                                        SET search_data = b.title || ' ' || coalesce((
                                          SELECT string_agg(c.title, ' ')
                                          FROM contents c
                                          JOIN book_content bc ON bc.content_id = c.id
                                          WHERE bc.book_id = b.id
                                        ), '')
                                        WHERE b.id = NEW.book_id;
                                      END IF;

                                      RETURN NEW;
                                    END;
                                    $$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(@"CREATE TRIGGER trg_book_content_search  
                                   AFTER INSERT OR UPDATE OR DELETE ON book_content
                                   FOR EACH ROW EXECUTE FUNCTION update_book_data_vector();");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION update_books_by_content()
                                    RETURNS TRIGGER AS $$
                                    BEGIN
                                      -- Находим все книги, связанные с этим контентом, и обновляем их
                                      UPDATE books b
                                      SET search_data = b.title || ' ' || coalesce((
                                        SELECT string_agg(c.title, ' ')
                                        FROM contents c
                                        JOIN book_content bc ON bc.content_id = c.id
                                        WHERE bc.book_id = b.id
                                      ), '')
                                      WHERE b.id IN (
                                        SELECT book_id FROM book_content WHERE content_id = NEW.id
                                      );
                                      RETURN NEW;
                                    END;
                                    $$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(@"CREATE TRIGGER trg_contents_search
                                   AFTER UPDATE OF title ON contents
                                   FOR EACH ROW EXECUTE FUNCTION update_books_by_content();");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION update_book_self()
                                    RETURNS TRIGGER AS $$
                                    BEGIN
                                      UPDATE books b
                                      SET search_data = b.title || ' ' || coalesce((
                                        SELECT string_agg(c.title, ' ')
                                        FROM contents c
                                        JOIN book_content bc ON bc.content_id = c.id
                                        WHERE bc.book_id = b.id
                                      ), '')
                                      WHERE b.id = NEW.id;
                                      RETURN NEW;
                                    END;
                                    $$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(@"CREATE TRIGGER trg_books_title_change 
                                   AFTER UPDATE OF title ON books
                                   FOR EACH ROW EXECUTE FUNCTION update_book_self();");
        }
    }
}
