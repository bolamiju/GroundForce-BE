using Microsoft.EntityFrameworkCore.Migrations;

namespace Groundforce.Services.Data.Migrations
{
    public partial class modifyVerify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestFailedCount",
                table: "Request");

            migrationBuilder.AddColumn<bool>(
                name: "Block",
                table: "Request",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RequestAttempt",
                table: "Request",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Block",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "RequestAttempt",
                table: "Request");

            migrationBuilder.AddColumn<int>(
                name: "RequestFailedCount",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
