using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTriggersForthemesAndTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION check_theme_cycle()
            RETURNS TRIGGER AS $$
            DECLARE
               current_id INT;
            BEGIN
               IF NEW.parent_theme_id IS NULL THEN
                  RETURN NEW;
               END IF;

               IF NEW.parent_theme_id = NEW.id THEN
                  RAISE EXCEPTION 'Тема не может быть родителем самой себя' USING ERRCODE=45002;
               END IF;
               
               current_id := NEW.parent_theme_id;

               WHILE current_id IS NOT NULL LOOP
                  IF current_id = NEW.id THEN
                     RAISE EXCEPTION 'Обнаружен цикл тем' USING ERRCODE=45002;
                  END IF;

                  SELECT parent_theme_id INTO current_id
                  FROM themes
                  WHERE id = current_id;
               END LOOP;

               RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(@"
            CREATE OR REPLACE TRIGGER trg_check_theme_cycle
            BEFORE INSERT OR UPDATE OF parent_theme_id ON themes
            FOR EACH ROW
            EXECUTE FUNCTION check_theme_cycle();");

            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION check_tag_cycle()
            RETURNS TRIGGER AS $$
            DECLARE
               current_id INT;
            BEGIN
               IF NEW.parent_tag_id IS NULL THEN
                  RETURN NEW;
               END IF;

               IF NEW.parent_tag_id = NEW.id THEN
                  RAISE EXCEPTION 'Тег не может быть родителем самого себя' USING ERRCODE=45003;
               END IF;

               current_id := NEW.parent_tag_id;

               WHILE current_id IS NOT NULL LOOP
                  IF current_id = NEW.id THEN
                     RAISE EXCEPTION 'Обнаружен цикл тегов' USING ERRCODE=45003;
                  END IF;

                  SELECT parent_tag_id INTO current_id
                  FROM tags
                  WHERE id = current_id;
               END LOOP;

               RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(@"
            CREATE OR REPLACE TRIGGER trg_check_tag_cycle
            BEFORE INSERT OR UPDATE OF parent_tag_id ON tags
            FOR EACH ROW
            EXECUTE FUNCTION check_tag_cycle();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_check_tag_cycle ON tags;");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_check_theme_cycle ON themes;");

            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS check_tag_cycle();");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS check_theme_cycle();");

        }
    }
}
