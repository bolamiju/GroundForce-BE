using Microsoft.EntityFrameworkCore.Migrations;

namespace Groundforce.Services.Data.Migrations
{
    public partial class updateassignedAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AddressExists",
                table: "AssignedAddresses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "AssignedAddresses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "AssignedAddresses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeOfStructure",
                table: "AssignedAddresses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressExists",
                table: "AssignedAddresses");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "AssignedAddresses");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "AssignedAddresses");

            migrationBuilder.DropColumn(
                name: "TypeOfStructure",
                table: "AssignedAddresses");
        }
    }
}
