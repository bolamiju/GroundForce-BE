using Microsoft.EntityFrameworkCore.Migrations;

namespace Groundforce.Services.Data.Migrations
{
    public partial class UpdateFieldAgentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalPhoneNumber",
                table: "FieldAgents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Religion",
                table: "FieldAgents",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalPhoneNumber",
                table: "FieldAgents");

            migrationBuilder.DropColumn(
                name: "Religion",
                table: "FieldAgents");
        }
    }
}
