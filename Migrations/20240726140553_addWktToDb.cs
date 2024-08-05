using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MapApplication.Migrations
{
    /// <inheritdoc />
    public partial class addWktToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "X",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "Y",
                table: "Points");

            migrationBuilder.AddColumn<string>(
                name: "Wkt",
                table: "Points",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "Wkt" },
                values: new object[] { "AreaTest", "POLYGON((10.689 -25.092, 34.595 -20.170, 38.814 -35.639, 13.502 -39.155, 10.689 -25.092))" });

            migrationBuilder.UpdateData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Wkt" },
                values: new object[] { "PointTEst", "POINT((34, 34))" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Wkt",
                table: "Points");

            migrationBuilder.AddColumn<double>(
                name: "X",
                table: "Points",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Y",
                table: "Points",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Polygons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Wkt = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polygons", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "X", "Y" },
                values: new object[] { "FromDbAnkara", 123.0, 456.0 });

            migrationBuilder.UpdateData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "X", "Y" },
                values: new object[] { "Bursa", 345.0, 567.0 });

            migrationBuilder.InsertData(
                table: "Polygons",
                columns: new[] { "Id", "Name", "Wkt" },
                values: new object[] { 1, "AreaTest", "POLYGON((10.689 -25.092, 34.595 -20.170, 38.814 -35.639, 13.502 -39.155, 10.689 -25.092))" });
        }
    }
}
