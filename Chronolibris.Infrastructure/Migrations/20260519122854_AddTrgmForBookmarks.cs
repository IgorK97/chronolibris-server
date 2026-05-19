using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrgmForBookmarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ix_bookmarks_context_trgm
                ON bookmarks
                USING gin (context gin_trgm_ops);
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ix_bookmarks_note_trgm
                ON bookmarks
                USING gin (note gin_trgm_ops)
                WHERE note IS NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ix_bookmarks_context_trgm;");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ix_bookmarks_note_trgm;");
        }
    }
}
