using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JetAdminSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingAndBrokerRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Billings_BookingId",
                table: "Billings");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Brokers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "IX_Billings_BookingId",
                table: "Billings",
                column: "BookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Billings_BookingId",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Brokers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_Billings_BookingId",
                table: "Billings",
                column: "BookingId",
                unique: true);
        }
    }
}
