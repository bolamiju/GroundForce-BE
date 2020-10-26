using Microsoft.EntityFrameworkCore.Migrations;

namespace Groundforce.Services.Data.Migrations
{
    public partial class RedoClientModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerificationItems_Clients_ClientId",
                table: "VerificationItems");

            migrationBuilder.DropIndex(
                name: "IX_VerificationItems_ClientId",
                table: "VerificationItems");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "VerificationItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "VerificationItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationItems_ClientId",
                table: "VerificationItems",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationItems_Clients_ClientId",
                table: "VerificationItems",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
