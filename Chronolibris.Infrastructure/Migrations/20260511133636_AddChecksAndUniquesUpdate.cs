using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChecksAndUniquesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropCheckConstraint(
            //    name: "ck_languages_name_not_empty",
            //    table: "languages");

            //migrationBuilder.AddCheckConstraint(
            //    name: "ck_languages_name_not_empty",
            //    table: "languages",
            //    sql: "LENGTH(TRIM(name))>0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropCheckConstraint(
            //    name: "ck_languages_name_not_empty",
            //    table: "languages");

            //migrationBuilder.AddCheckConstraint(
            //    name: "ck_languages_name_not_empty",
            //    table: "languages",
            //    sql: "name = LENGTH(TRIM(name))>0");
        }
    }
}
