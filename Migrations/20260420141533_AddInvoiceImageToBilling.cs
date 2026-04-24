using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JetAdminSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceImageToBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceImageUrl",
                table: "Billings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Billings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceImageUrl",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Billings");
        }
    }
}
