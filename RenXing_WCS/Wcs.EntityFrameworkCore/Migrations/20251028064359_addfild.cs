using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Migrations
{
    public partial class addfild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmptyStationNo",
                table: "dispatchorders",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmptyStationNo",
                table: "dispatchorders");
        }
    }
}
