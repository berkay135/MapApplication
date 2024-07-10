using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapApplication.Migrations
{
    /// <inheritdoc />
    public partial class changeSeededDataName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "FromDbAnkara");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Ankara");
        }
    }
}
