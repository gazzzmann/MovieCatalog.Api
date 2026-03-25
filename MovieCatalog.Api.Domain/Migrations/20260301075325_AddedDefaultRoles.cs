using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieCatalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "06209a3b-00eb-442b-b487-45ab183f3729", "6fb1918e-5dde-477f-84cc-d951c30efddb", "User", "USER" },
                    { "8e653da4-c1c7-4c7d-b0bc-0a62fd6bd4a1", "8f011ee2-e9e7-4351-bbbb-72dd8fb9a611", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "06209a3b-00eb-442b-b487-45ab183f3729");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8e653da4-c1c7-4c7d-b0bc-0a62fd6bd4a1");
        }
    }
}
