using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronolibris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewContentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "content_type",
                columns: new[] { "id", "name", "nature" },
                values: new object[] { 23L, "Комментарий", ContentNature.Analysis });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "content_type",
                keyColumn: "id",
                keyValue: 23L);
        }
    }
}
