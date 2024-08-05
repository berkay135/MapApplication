using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapApplication.Migrations
{
    /// <inheritdoc />
    public partial class typo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "PointTest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "PointTEst");
        }
    }
}
