using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Migrations
{
    public partial class init6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmptyStationNo",
                table: "dispatchorders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmptyStationNo",
                table: "dispatchorders",
                type: "TEXT",
                nullable: true);
        }
    }
}
