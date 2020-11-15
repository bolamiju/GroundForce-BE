﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Groundforce.Services.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    Gender = table.Column<string>(maxLength: 1, nullable: false),
                    DOB = table.Column<string>(maxLength: 10, nullable: false),
                    IsVerified = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    AvatarUrl = table.Column<string>(nullable: true),
                    PublicId = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BuildingTypes",
                columns: table => new
                {
                    TypeId = table.Column<string>(nullable: false),
                    TypeName = table.Column<string>(maxLength: 35, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingTypes", x => x.TypeId);
                });

            migrationBuilder.CreateTable(
                name: "Points",
                columns: table => new
                {
                    PointId = table.Column<string>(nullable: false),
                    PointNumber = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Points", x => x.PointId);
                });

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    RequestId = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    RequestAttempt = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.RequestId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldAgents",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(nullable: false),
                    PlaceOfBirth = table.Column<string>(maxLength: 150, nullable: false),
                    State = table.Column<string>(maxLength: 150, nullable: false),
                    LGA = table.Column<string>(maxLength: 150, nullable: false),
                    HomeAddress = table.Column<string>(maxLength: 200, nullable: false),
                    Longitude = table.Column<string>(nullable: false),
                    Latitude = table.Column<string>(nullable: false),
                    Religion = table.Column<string>(maxLength: 25, nullable: false),
                    AdditionalPhoneNumber = table.Column<string>(maxLength: 14, nullable: true),
                    AccountName = table.Column<string>(maxLength: 100, nullable: false),
                    AccountNumber = table.Column<string>(maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldAgents", x => x.ApplicationUserId);
                    table.ForeignKey(
                        name: "FK_FieldAgents_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserActivity",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    RecordId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivity_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerificationItems",
                columns: table => new
                {
                    ItemId = table.Column<string>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false),
                    Title = table.Column<string>(maxLength: 20, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationItems", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_VerificationItems_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointAllocated",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    FieldAgentId = table.Column<string>(nullable: false),
                    PointsId = table.Column<string>(nullable: false),
                    PointId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointAllocated", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointAllocated_FieldAgents_FieldAgentId",
                        column: x => x.FieldAgentId,
                        principalTable: "FieldAgents",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PointAllocated_Points_PointId",
                        column: x => x.PointId,
                        principalTable: "Points",
                        principalColumn: "PointId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    FieldAgentId = table.Column<string>(nullable: false),
                    Reference = table.Column<string>(nullable: false),
                    PaidAmount = table.Column<int>(nullable: false),
                    PaidAt = table.Column<DateTime>(nullable: false),
                    ActualAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_FieldAgents_FieldAgentId",
                        column: x => x.FieldAgentId,
                        principalTable: "FieldAgents",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    MissionId = table.Column<string>(nullable: false),
                    VerificationItemId = table.Column<string>(nullable: false),
                    FieldAgentId = table.Column<string>(nullable: false),
                    VerificationStatus = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.MissionId);
                    table.ForeignKey(
                        name: "FK_Missions_FieldAgents_FieldAgentId",
                        column: x => x.FieldAgentId,
                        principalTable: "FieldAgents",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Missions_VerificationItems_VerificationItemId",
                        column: x => x.VerificationItemId,
                        principalTable: "VerificationItems",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateTable(
                name: "MissionsVerified",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    MissionId = table.Column<string>(nullable: true),
                    BuildingTypeId = table.Column<string>(nullable: true),
                    Landmark = table.Column<string>(maxLength: 150, nullable: true),
                    BusStop = table.Column<string>(maxLength: 150, nullable: true),
                    BuildingColor = table.Column<string>(maxLength: 20, nullable: true),
                    AddressExists = table.Column<bool>(nullable: false),
                    TypeOfStructure = table.Column<string>(maxLength: 35, nullable: true),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionsVerified", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissionsVerified_BuildingTypes_BuildingTypeId",
                        column: x => x.BuildingTypeId,
                        principalTable: "BuildingTypes",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MissionsVerified_Missions_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Missions",
                        principalColumn: "MissionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_FieldAgentId",
                table: "Missions",
                column: "FieldAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_VerificationItemId",
                table: "Missions",
                column: "VerificationItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MissionsVerified_BuildingTypeId",
                table: "MissionsVerified",
                column: "BuildingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionsVerified_MissionId",
                table: "MissionsVerified",
                column: "MissionId",
                unique: true,
                filter: "[MissionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PointAllocated_FieldAgentId",
                table: "PointAllocated",
                column: "FieldAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_PointAllocated_PointId",
                table: "PointAllocated",
                column: "PointId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_FieldAgentId",
                table: "Transactions",
                column: "FieldAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivity_ApplicationUserId",
                table: "UserActivity",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationItems_ApplicationUserId",
                table: "VerificationItems",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "MissionsVerified");

            migrationBuilder.DropTable(
                name: "PointAllocated");

            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "UserActivity");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "BuildingTypes");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "Points");

            migrationBuilder.DropTable(
                name: "FieldAgents");

            migrationBuilder.DropTable(
                name: "VerificationItems");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
