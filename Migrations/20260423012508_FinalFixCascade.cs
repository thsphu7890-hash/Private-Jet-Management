using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JetAdminSystem.Migrations
{
    /// <inheritdoc />
    public partial class FinalFixCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightSchedules_Airports_ArrivalAirportId",
                table: "FlightSchedules");

            migrationBuilder.AddColumn<string>(
                name: "PassportScanUrl",
                table: "Passengers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Aircrafts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightSchedules_Airports_ArrivalAirportId",
                table: "FlightSchedules",
                column: "ArrivalAirportId",
                principalTable: "Airports",
                principalColumn: "AirportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightSchedules_Airports_ArrivalAirportId",
                table: "FlightSchedules");

            migrationBuilder.DropColumn(
                name: "PassportScanUrl",
                table: "Passengers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Aircrafts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightSchedules_Airports_ArrivalAirportId",
                table: "FlightSchedules",
                column: "ArrivalAirportId",
                principalTable: "Airports",
                principalColumn: "AirportId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
