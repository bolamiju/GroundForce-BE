using Microsoft.EntityFrameworkCore.Migrations;

namespace Groundforce.Services.Data.Migrations
{
    public partial class RequestUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Block",
                table: "Request");

            migrationBuilder.AddColumn<bool>(
                name: "IsBlock",
                table: "Request",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlock",
                table: "Request");

            migrationBuilder.AddColumn<bool>(
                name: "Block",
                table: "Request",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
