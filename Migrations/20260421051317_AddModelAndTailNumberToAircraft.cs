using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JetAdminSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddModelAndTailNumberToAircraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_Bookings_BookingId",
                table: "Passengers");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_BookingId",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerPhone",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "PassengerId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Aircrafts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TailNumber",
                table: "Aircrafts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PassengerId",
                table: "Bookings",
                column: "PassengerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Passengers_PassengerId",
                table: "Bookings",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "PassengerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Passengers_PassengerId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PassengerId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PassengerId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Aircrafts");

            migrationBuilder.DropColumn(
                name: "TailNumber",
                table: "Aircrafts");

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Passengers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Bookings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhone",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Bookings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_BookingId",
                table: "Passengers",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_Bookings_BookingId",
                table: "Passengers",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
