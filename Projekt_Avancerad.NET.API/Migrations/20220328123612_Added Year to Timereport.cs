using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projekt_Avancerad.NET.API.Migrations
{
    public partial class AddedYeartoTimereport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "TimeReports",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "TimeReports");
        }
    }
}
