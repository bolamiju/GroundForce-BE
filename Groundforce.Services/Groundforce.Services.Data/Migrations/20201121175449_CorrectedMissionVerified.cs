using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Groundforce.Services.Data.Migrations
{
    public partial class CorrectedMissionVerified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissionsVerified_BuildingTypes_BuildingTypeId",
                table: "MissionsVerified");

            migrationBuilder.DropIndex(
                name: "IX_MissionsVerified_MissionId",
                table: "MissionsVerified");

            migrationBuilder.AlterColumn<string>(
                name: "TypeOfStructure",
                table: "MissionsVerified",
                maxLength: 35,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(35)",
                oldMaxLength: 35,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "MissionsVerified",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MissionId",
                table: "MissionsVerified",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Longitude",
                table: "MissionsVerified",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                table: "MissionsVerified",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Landmark",
                table: "MissionsVerified",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BusStop",
                table: "MissionsVerified",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BuildingTypeId",
                table: "MissionsVerified",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BuildingColor",
                table: "MissionsVerified",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MissionsVerified",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MissionsVerified",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_MissionsVerified_MissionId",
                table: "MissionsVerified",
                column: "MissionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MissionsVerified_BuildingTypes_BuildingTypeId",
                table: "MissionsVerified",
                column: "BuildingTypeId",
                principalTable: "BuildingTypes",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissionsVerified_BuildingTypes_BuildingTypeId",
                table: "MissionsVerified");

            migrationBuilder.DropIndex(
                name: "IX_MissionsVerified_MissionId",
                table: "MissionsVerified");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MissionsVerified");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MissionsVerified");

            migrationBuilder.AlterColumn<string>(
                name: "TypeOfStructure",
                table: "MissionsVerified",
                type: "nvarchar(35)",
                maxLength: 35,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 35);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "MissionsVerified",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "MissionId",
                table: "MissionsVerified",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Longitude",
                table: "MissionsVerified",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                table: "MissionsVerified",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Landmark",
                table: "MissionsVerified",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "BusStop",
                table: "MissionsVerified",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "BuildingTypeId",
                table: "MissionsVerified",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "BuildingColor",
                table: "MissionsVerified",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_MissionsVerified_MissionId",
                table: "MissionsVerified",
                column: "MissionId",
                unique: true,
                filter: "[MissionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_MissionsVerified_BuildingTypes_BuildingTypeId",
                table: "MissionsVerified",
                column: "BuildingTypeId",
                principalTable: "BuildingTypes",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
