using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JetAdminSystem.Migrations
{
    /// <inheritdoc />
    public partial class FinalizeBookingRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aircrafts_AircraftCategories_CategoryId",
                table: "Aircrafts");

            migrationBuilder.DropIndex(
                name: "IX_Billings_BookingId",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Accounts",
                newName: "Password");

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

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_Billings_BookingId",
                table: "Billings",
                column: "BookingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Aircrafts_AircraftCategories_CategoryId",
                table: "Aircrafts",
                column: "CategoryId",
                principalTable: "AircraftCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aircrafts_AircraftCategories_CategoryId",
                table: "Aircrafts");

            migrationBuilder.DropIndex(
                name: "IX_Billings_BookingId",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerPhone",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Accounts",
                newName: "PasswordHash");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Accounts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Accounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "Accounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Billings_BookingId",
                table: "Billings",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Aircrafts_AircraftCategories_CategoryId",
                table: "Aircrafts",
                column: "CategoryId",
                principalTable: "AircraftCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
