using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_review_rating",
                table: "reviews");

            migrationBuilder.AlterColumn<DateTime>(
                name: "registered_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "selections",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "reviews",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "reports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "started_at",
                table: "moderation_tasks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "contents",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "comments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "books",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "bookmarks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "book_fragments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "book_files",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddCheckConstraint(
                name: "ck_review_rating",
                table: "reviews",
                sql: "score IN (1,2,3,4,5)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_contents_description_min_length",
                table: "contents",
                sql: "LENGTH(description)>=120");

            migrationBuilder.AddCheckConstraint(
                name: "CK_contents_title_alnum",
                table: "contents",
                sql: "title ~ '[[:alnum:]]'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_contents_years",
                table: "contents",
                sql: "(year_from is null AND year_to is null) OR (year_from >= -10000 AND year_from <= year_to AND year_to <= EXTRACT(YEAR FROM CURRENT_TIMESTAMP)+1)OR (year_from is null AND -10000 <= year_to AND year_to <= EXTRACT(YEAR FROM CURRENT_TIMESTAMP)+1)OR (year_to is null AND -10000 <= year_from AND year_from <= EXTRACT(YEAR FROM CURRENT_TIMESTAMP)+1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_books_description_min_length",
                table: "books",
                sql: "LENGTH(description) >= 120");

            migrationBuilder.AddCheckConstraint(
                name: "CK_books_title_alnum",
                table: "books",
                sql: "title ~ '[[:alnum:]]'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_books_year_range",
                table: "books",
                sql: "year IS NULL OR (year>=-10000 AND year <= EXTRACT(YEAR FROM CURRENT_DATE)+1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_book_fragments_position_positive",
                table: "book_fragments",
                sql: "position >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_book_fragments_start_end_pos_positive",
                table: "book_fragments",
                sql: "end_pos >= start_pos AND start_pos >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_book_files_original_size_positive",
                table: "book_files",
                sql: "original_size > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_book_files_stored_size_not_negative",
                table: "book_files",
                sql: "stored_size>=0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_review_rating",
                table: "reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_contents_description_min_length",
                table: "contents");

            migrationBuilder.DropCheckConstraint(
                name: "CK_contents_title_alnum",
                table: "contents");

            migrationBuilder.DropCheckConstraint(
                name: "CK_contents_years",
                table: "contents");

            migrationBuilder.DropCheckConstraint(
                name: "CK_books_description_min_length",
                table: "books");

            migrationBuilder.DropCheckConstraint(
                name: "CK_books_title_alnum",
                table: "books");

            migrationBuilder.DropCheckConstraint(
                name: "CK_books_year_range",
                table: "books");

            migrationBuilder.DropCheckConstraint(
                name: "CK_book_fragments_position_positive",
                table: "book_fragments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_book_fragments_start_end_pos_positive",
                table: "book_fragments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_book_files_original_size_positive",
                table: "book_files");

            migrationBuilder.DropCheckConstraint(
                name: "CK_book_files_stored_size_not_negative",
                table: "book_files");

            migrationBuilder.AlterColumn<DateTime>(
                name: "registered_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "selections",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "reviews",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "reports",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "started_at",
                table: "moderation_tasks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "contents",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "comments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "books",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "bookmarks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "book_fragments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "book_files",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddCheckConstraint(
                name: "ck_review_rating",
                table: "reviews",
                sql: "score >=0.0 AND score<=5.0");
        }
    }
}
