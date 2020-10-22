using Microsoft.EntityFrameworkCore.Migrations;

namespace Groundforce.Services.Data.Migrations
{
    public partial class FKFixMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AssignedAddresses_AddressId",
                table: "Addresses");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedAddresses_AddressId",
                table: "AssignedAddresses",
                column: "AddressId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedAddresses_Addresses_AddressId",
                table: "AssignedAddresses",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "AddressId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedAddresses_Addresses_AddressId",
                table: "AssignedAddresses");

            migrationBuilder.DropIndex(
                name: "IX_AssignedAddresses_AddressId",
                table: "AssignedAddresses");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AssignedAddresses_AddressId",
                table: "Addresses",
                column: "AddressId",
                principalTable: "AssignedAddresses",
                principalColumn: "Id");
        }
    }
}
