using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChecksAndUniques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tags_tag_relation_type_relation_type_id",
                table: "tags");

            //migrationBuilder.DropIndex(
            //    name: "ix_reviews_user_id_book_id",
            //    table: "reviews");

            migrationBuilder.DropCheckConstraint(
                name: "ck_languages_name_not_empty",
                table: "languages");

            //migrationBuilder.DeleteData(
            //    table: "user_role",
            //    keyColumns: new[] { "role_id", "user_id" },
            //    keyValues: new object[] { 1L, 1L });

            migrationBuilder.CreateIndex(
                name: "ix_reviews_user_id_book_id",
                table: "reviews",
                columns: new[] { "user_id", "book_id" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.AddCheckConstraint(
                name: "ck_languages_name_not_empty",
                table: "languages",
                sql: "LENGTH(TRIM(name))>0");

            migrationBuilder.CreateIndex(
                name: "ix_formats_name",
                table: "formats",
                column: "name",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_formats_name_clean",
                table: "formats",
                sql: "name = LOWER(TRIM(name)) AND LENGTH(TRIM(name)) > 0");

            migrationBuilder.AddForeignKey(
                name: "fk_tags_tag_relation_type_relation_type_id",
                table: "tags",
                column: "relation_type_id",
                principalTable: "tag_relation_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(
                @"CREATE UNIQUE INDEX ix_countries_name_unique ON countries (LOWER(TRIM(name)));");

            migrationBuilder.Sql(
                @"CREATE UNIQUE INDEX ix_languages_name_unique ON languages (LOWER(TRIM(name)));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"DROP INDEX ix_countries_name_unique");

            migrationBuilder.Sql(
                @"DROP INDEX ix_languages_name_unique");

            migrationBuilder.DropForeignKey(
                name: "fk_tags_tag_relation_type_relation_type_id",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_reviews_user_id_book_id",
                table: "reviews");

            migrationBuilder.DropCheckConstraint(
                name: "ck_languages_name_not_empty",
                table: "languages");

            migrationBuilder.DropIndex(
                name: "ix_formats_name",
                table: "formats");

            migrationBuilder.DropCheckConstraint(
                name: "ck_formats_name_clean",
                table: "formats");

            //migrationBuilder.InsertData(
            //    table: "user_role",
            //    columns: new[] { "role_id", "user_id" },
            //    values: new object[] { 1L, 1L });

            //migrationBuilder.CreateIndex(
            //    name: "ix_reviews_user_id_book_id",
            //    table: "reviews",
            //    columns: new[] { "user_id", "book_id" },
            //    unique: true,
            //    filter: "\"review_status_id\" != 4");

            migrationBuilder.AddCheckConstraint(
                name: "ck_languages_name_not_empty",
                table: "languages",
                sql: "LENGTH(TRIM(name))>0");

            migrationBuilder.AddForeignKey(
                name: "fk_tags_tag_relation_type_relation_type_id",
                table: "tags",
                column: "relation_type_id",
                principalTable: "tag_relation_type",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
