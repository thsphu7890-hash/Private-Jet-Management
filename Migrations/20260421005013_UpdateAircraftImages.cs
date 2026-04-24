using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JetAdminSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAircraftImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Aircrafts",
                newName: "InteriorImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "ExteriorImageUrl",
                table: "Aircrafts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExteriorImageUrl",
                table: "Aircrafts");

            migrationBuilder.RenameColumn(
                name: "InteriorImageUrl",
                table: "Aircrafts",
                newName: "ImageUrl");
        }
    }
}
