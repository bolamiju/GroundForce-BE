using Microsoft.EntityFrameworkCore.Migrations;

namespace Groundforce.Services.Data.Migrations
{
    public partial class addPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Religion",
                table: "FieldAgents",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Religion",
                table: "FieldAgents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
