using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePersonRolesAddChecksAddHistoricalTextManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_book_files_book_id_format_id",
                table: "book_files");

            migrationBuilder.DropIndex(
                name: "ix_book_files_book_id_is_readable",
                table: "book_files");

            migrationBuilder.AddColumn<bool>(
                name: "has_historical_versions",
                table: "books",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "historical_text",
                table: "book_files",
                type: "boolean",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 6L,
                column: "name",
                value: "Корректор (редактор подготовки текста)");

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_first_name_correct_length",
                table: "users",
                sql: "LENGTH(TRIM(first_name))>0 AND LENGTH(TRIM(first_name))<65");

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_last_name_correct_length",
                table: "users",
                sql: "LENGTH(TRIM(last_name))>0 AND LENGTH(TRIM(last_name))<65");

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_user_name_correct_length",
                table: "users",
                sql: "LENGTH(TRIM(user_name))>4 AND LENGTH(TRIM(user_name))<33");

            migrationBuilder.AddCheckConstraint(
                name: "ck_themes_name_not_empty",
                table: "themes",
                sql: "LENGTH(TRIM(name))>0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_tags_name_not_empty",
                table: "tags",
                sql: "LENGTH(TRIM(name))>0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_shelves_name_not_empty",
                table: "shelves",
                sql: "LENGTH(TRIM(name))>0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_publishers_name_not_empty",
                table: "publishers",
                sql: "LENGTH(TRIM(name))>0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_persons_name_not_empty",
                table: "persons",
                sql: "LENGTH(TRIM(name))>0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_languages_name_not_empty",
                table: "languages",
                sql: "LENGTH(TRIM(name))>0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_countries_name_not_empty",
                table: "countries",
                sql: "LENGTH(TRIM(name))>0");

            migrationBuilder.CreateIndex(
                name: "ix_book_files_book_id_format_id_historical_text",
                table: "book_files",
                columns: new[] { "book_id", "format_id", "historical_text" },
                unique: true);

            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION check_file_consistency()
            RETURNS TRIGGER AS $$
            DECLARE
               v_has_historical BOOLEAN;
            BEGIN
               SELECT has_historical_versions INTO v_has_historical
               FROM books
               WHERE id = NEW.book_id;

               IF v_has_historical AND NEW.historical_text IS NULL THEN
                  RAISE EXCEPTION 'Для книги с историческими версиями нужно указать тип текста';
               END IF;

               IF NOT v_has_historical AND NEW.historical_text IS NOT NULL THEN
                  RAISE EXCEPTION 'Для обычной книги historical_text должно быть равно NULL';
               END IF;

               RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(@"
            CREATE TRIGGER trigger_check_file_consistency
            BEFORE INSERT OR UPDATE ON book_files
            FOR EACH ROW EXECUTE FUNCTION check_file_consistency();");

            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION check_book_historical_change()
            RETURNS TRIGGER AS $$
            BEGIN
               IF(OLD.has_historical_versions IS DISTINCT FROM NEW.has_historical_versions) THEN

               IF NEW.has_historical_versions = TRUE AND EXISTS (
                  SELECT 1 FROM book_files WHERE book_id = NEW.id AND historical_text IS NULL
               ) THEN 
                  RAISE EXCEPTION 'Нельзя включить исторический режим: у книги уже есть книги без указания типа текста';
               END IF;

               IF NEW.has_historical_versions = FALSE AND EXISTS (
                  SELECT 1 FROM book_files WHERE book_id = NEW.id AND historical_text IS NOT NULL
               ) THEN
                  RAISE EXCEPTION 'Нельзя отключить исторический режим: у книги есть файлы с указанием типа текста';
               END IF;

               END IF;

               RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(@"
            CREATE TRIGGER trigger_check_book_historical_change
            BEFORE INSERT OR UPDATE ON books
            FOR EACH ROW EXECUTE FUNCTION check_book_historical_change();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trigger_check_file_consistency ON book_files;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS check_file_consistency();");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trigger_check_book_historical_change ON books;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS check_book_historical_change();");


            migrationBuilder.DropCheckConstraint(
                name: "ck_users_first_name_correct_length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "ck_users_last_name_correct_length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "ck_users_user_name_correct_length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "ck_themes_name_not_empty",
                table: "themes");

            migrationBuilder.DropCheckConstraint(
                name: "ck_tags_name_not_empty",
                table: "tags");

            migrationBuilder.DropCheckConstraint(
                name: "ck_shelves_name_not_empty",
                table: "shelves");

            migrationBuilder.DropCheckConstraint(
                name: "ck_publishers_name_not_empty",
                table: "publishers");

            migrationBuilder.DropCheckConstraint(
                name: "ck_persons_name_not_empty",
                table: "persons");

            migrationBuilder.DropCheckConstraint(
                name: "ck_languages_name_not_empty",
                table: "languages");

            migrationBuilder.DropCheckConstraint(
                name: "ck_countries_name_not_empty",
                table: "countries");

            migrationBuilder.DropIndex(
                name: "ix_book_files_book_id_format_id_historical_text",
                table: "book_files");

            migrationBuilder.DropColumn(
                name: "has_historical_versions",
                table: "books");

            migrationBuilder.DropColumn(
                name: "historical_text",
                table: "book_files");

            migrationBuilder.UpdateData(
                table: "person_roles",
                keyColumn: "id",
                keyValue: 6L,
                column: "name",
                value: "Корректор");

            migrationBuilder.CreateIndex(
                name: "ix_book_files_book_id_format_id",
                table: "book_files",
                columns: new[] { "book_id", "format_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_book_files_book_id_is_readable",
                table: "book_files",
                columns: new[] { "book_id", "is_readable" },
                unique: true,
                filter: "\"is_readable\" = true");
        }
    }
}
