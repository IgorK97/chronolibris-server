using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION check_unique_phone_per_role()
            RETURNS TRIGGER AS $$
            BEGIN
               IF EXISTS (
                  SELECT 1
                  FROM users u
                  JOIN user_role ur ON u.id = ur.user_id
                  WHERE u.phone_number = (SELECT phone_number FROM users WHERE id = NEW.user_id)
                     AND ur.role_id = NEW.role_id
                     AND u.id != NEW.user_id
               ) THEN
                  RAISE EXCEPTION 'Данный номер телефона уже используется аккаунтом с такой же ролью';
               END IF;
               RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(@"
            CREATE TRIGGER trg_unique_phone_role
            BEFORE INSERT ON user_role
            FOR EACH ROW EXECUTE FUNCTION check_unique_phone_per_role();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_unique_phone_role;
            DROP FUNCTION IF EXISTS check_unique_phone_per_role();");
        }
    }
}
