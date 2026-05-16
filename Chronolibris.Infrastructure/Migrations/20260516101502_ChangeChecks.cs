using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_users_first_name_correct_length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "ck_users_last_name_correct_length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "ck_users_user_name_correct_length",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_first_name_correct_length",
                table: "users",
                sql: "LENGTH(TRIM(first_name))>0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_last_name_correct_length",
                table: "users",
                sql: "LENGTH(TRIM(last_name))>0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_user_name_correct_length",
                table: "users",
                sql: "LENGTH(TRIM(user_name))>4");

            migrationBuilder.CreateIndex(
                name: "ix_user_role_user_id",
                table: "user_role",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_users_first_name_correct_length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "ck_users_last_name_correct_length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "ck_users_user_name_correct_length",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_user_role_user_id",
                table: "user_role");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

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
        }
    }
}
