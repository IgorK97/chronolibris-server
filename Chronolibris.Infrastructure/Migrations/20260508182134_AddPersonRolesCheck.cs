using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonRolesCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION check_participation_role_kind()
            RETURNS TRIGGER AS $$
            DECLARE
               role_kind person_role_kind;
               entity_type TEXT := TG_ARGV[0];
            BEGIN
               SELECT kind INTO role_kind
               FROM person_roles
               WHERE id = NEW.person_role_id;
               
               IF role_kind IS NULL THEN
                  RAISE EXCEPTION 'Роль с id % не найдена', NEW.person_role_id;
               END IF;
               
               IF entity_type = 'content' AND role_kind = 'book' THEN
                  RAISE EXCEPTION
                     'Роль с id % (%) неприменима к контенту', NEW.person_role_id, role_kind
                     USING ERRCODE = 'check_violation';
               END IF;
               
               IF entity_type = 'book' AND role_kind = 'content' THEN
               RAISE EXCEPTION
                  'Роль с id % (%) неприменима к книге', NEW.person_role_id, role_kind
                  USING ERRCODE = 'check_violation';
               END IF;

               RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(@"
            CREATE TRIGGER trg_content_participation_role_kind
            BEFORE INSERT OR UPDATE ON content_participations
            FOR EACH ROW
            EXECUTE FUNCTION check_participation_role_kind('content');");

            migrationBuilder.Sql(@"
            CREATE TRIGGER trg_book_participation_role_kind
            BEFORE INSERT OR UPDATE ON book_participations
            FOR EACH ROW
            EXECUTE FUNCTION check_participation_role_kind('book');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP TRIGGER IF EXISTS trg_content_participation_role_kind ON content_participations;");

            migrationBuilder.Sql(
                "DROP TRIGGER IF EXISTS trg_book_participation_role_kind ON book_participations;");

            migrationBuilder.Sql(
                "DROP FUNCTION IF EXISTS check_participation_role_kind();");
        }
    }
}
