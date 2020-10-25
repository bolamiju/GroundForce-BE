using Microsoft.EntityFrameworkCore.Migrations;

namespace Groundforce.Services.Data.Migrations
{
    public partial class RequestUpdateMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Request");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Request",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Request");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Request",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
