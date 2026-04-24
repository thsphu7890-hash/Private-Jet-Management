using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JetAdminSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBillingFinancialFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Billings_BookingId",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Billings");

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "InternalNotes",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeatCount",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Surcharge",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Billings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Billings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "BrokerCommission",
                table: "Billings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Billings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionReference",
                table: "Billings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Billings_BookingId",
                table: "Billings",
                column: "BookingId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Billings_BookingId",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "InternalNotes",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "SeatCount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Surcharge",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BrokerCommission",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "TransactionReference",
                table: "Billings");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Billings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Billings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Billings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Billings_BookingId",
                table: "Billings",
                column: "BookingId");
        }
    }
}
