using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlcServer.Devices.DeviceServices.DeviceServiceByMySql.Migrations
{
    /// <inheritdoc />
    public partial class chg2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlcClassName",
                table: "PlcDevices",
                newName: "DriverClassName");

            migrationBuilder.AddColumn<string>(
                name: "DriverAssemblyName",
                table: "PlcDevices",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverAssemblyName",
                table: "PlcDevices");

            migrationBuilder.RenameColumn(
                name: "DriverClassName",
                table: "PlcDevices",
                newName: "PlcClassName");
        }
    }
}
