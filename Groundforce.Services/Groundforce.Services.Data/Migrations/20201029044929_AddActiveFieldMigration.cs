using Microsoft.EntityFrameworkCore.Migrations;

namespace Groundforce.Services.Data.Migrations
{
    public partial class AddActiveFieldMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccountActive",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccountActive",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
