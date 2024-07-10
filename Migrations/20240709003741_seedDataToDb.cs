using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MapApplication.Migrations
{
    /// <inheritdoc />
    public partial class seedDataToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Points",
                columns: new[] { "Id", "Name", "X", "Y" },
                values: new object[,]
                {
                    { 1, "Ankara", 123.0, 456.0 },
                    { 2, "Bursa", 345.0, 567.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
