using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlcServer.Devices.DeviceServices.DeviceServiceByMySql.Migrations
{
    /// <inheritdoc />
    public partial class chg1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverAssemblyName",
                table: "PlcDevices");

            migrationBuilder.DropColumn(
                name: "DriverClassName",
                table: "PlcDevices");

            migrationBuilder.AddColumn<string>(
                name: "PlcClassName",
                table: "PlcDevices",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlcClassName",
                table: "PlcDevices");

            migrationBuilder.AddColumn<string>(
                name: "DriverAssemblyName",
                table: "PlcDevices",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DriverClassName",
                table: "PlcDevices",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
