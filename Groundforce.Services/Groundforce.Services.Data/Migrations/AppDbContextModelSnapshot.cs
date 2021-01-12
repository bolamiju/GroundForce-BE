﻿// <auto-generated />
using System;
using Groundforce.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Groundforce.Services.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("Groundforce.Services.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("DOB")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(10);

                    b.Property<string>("Email")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Gender")
                        .HasColumnType("TEXT")
                        .HasMaxLength(1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PublicId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Groundforce.Services.Models.BuildingType", b =>
                {
                    b.Property<string>("TypeId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(35);

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("TypeId");

                    b.ToTable("BuildingTypes");
                });

            modelBuilder.Entity("Groundforce.Services.Models.EmailVerification", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("INTEGER");

                    b.Property<string>("VerificationCode")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(4);

                    b.HasKey("Id");

                    b.ToTable("EmailVerifications");
                });

            modelBuilder.Entity("Groundforce.Services.Models.FieldAgent", b =>
                {
                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AccountName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(100);

                    b.Property<string>("AccountNumber")
                        .HasColumnType("TEXT")
                        .HasMaxLength(10);

                    b.Property<string>("AdditionalPhoneNumber")
                        .HasColumnType("TEXT")
                        .HasMaxLength(14);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsLocationVerified")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LGA")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(150);

                    b.Property<string>("Latitude")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Longitude")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Religion")
                        .HasColumnType("TEXT")
                        .HasMaxLength(25);

                    b.Property<string>("ResidentialAddress")
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(150);

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(10);

                    b.HasKey("ApplicationUserId");

                    b.ToTable("FieldAgents");
                });

            modelBuilder.Entity("Groundforce.Services.Models.Mission", b =>
                {
                    b.Property<string>("MissionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AddedBy")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("FieldAgentId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.Property<string>("VerificationItemId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("VerificationStatus")
                        .HasColumnType("TEXT");

                    b.HasKey("MissionId");

                    b.HasIndex("FieldAgentId");

                    b.HasIndex("VerificationItemId")
                        .IsUnique();

                    b.ToTable("Missions");
                });

            modelBuilder.Entity("Groundforce.Services.Models.MissionVerified", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<bool>("AddressExists")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BuildingColor")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("BuildingTypeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("BusStop")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(150);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Landmark")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(150);

                    b.Property<string>("Latitude")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Longitude")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MissionId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Remarks")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TypeOfStructure")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(35);

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BuildingTypeId");

                    b.HasIndex("MissionId")
                        .IsUnique();

                    b.ToTable("MissionsVerified");
                });

            modelBuilder.Entity("Groundforce.Services.Models.Notification", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("AddedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notifications")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Groundforce.Services.Models.NotificationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ApplicationUserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NotificationId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("NotificationId");

                    b.ToTable("NotificationUsers");
                });

            modelBuilder.Entity("Groundforce.Services.Models.Point", b =>
                {
                    b.Property<string>("PointId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("PointNumber")
                        .HasColumnType("decimal(18,4)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("PointId");

                    b.ToTable("Points");
                });

            modelBuilder.Entity("Groundforce.Services.Models.PointAllocated", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("AddedBy")
                        .HasColumnType("TEXT");

                    b.Property<string>("FieldAgentId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PointId")
                        .HasColumnType("TEXT");

                    b.Property<string>("PointsId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FieldAgentId");

                    b.HasIndex("PointId");

                    b.ToTable("PointAllocated");
                });

            modelBuilder.Entity("Groundforce.Services.Models.QuestionOption", b =>
                {
                    b.Property<string>("QuestionOptionId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Option")
                        .HasColumnType("TEXT");

                    b.Property<string>("SurveyQuestionId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("QuestionOptionId");

                    b.HasIndex("SurveyQuestionId");

                    b.ToTable("QuestionOptions");
                });

            modelBuilder.Entity("Groundforce.Services.Models.Request", b =>
                {
                    b.Property<string>("RequestId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RequestAttempt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("RequestId");

                    b.ToTable("Request");
                });

            modelBuilder.Entity("Groundforce.Services.Models.Response", b =>
                {
                    b.Property<string>("ResponseId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApplicationUserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("QuestionOptionId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SurveyId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SurveyQuestionId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserSurveyApplicationUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserSurveySurveyId")
                        .HasColumnType("TEXT");

                    b.HasKey("ResponseId");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("QuestionOptionId");

                    b.HasIndex("SurveyId");

                    b.HasIndex("SurveyQuestionId");

                    b.HasIndex("UserSurveyApplicationUserId", "UserSurveySurveyId");

                    b.ToTable("Responses");
                });

            modelBuilder.Entity("Groundforce.Services.Models.Survey", b =>
                {
                    b.Property<string>("SurveyId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AddedBy")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("SurveyTypeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Topic")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("SurveyId");

                    b.HasIndex("SurveyTypeId");

                    b.ToTable("Surveys");
                });

            modelBuilder.Entity("Groundforce.Services.Models.SurveyQuestion", b =>
                {
                    b.Property<string>("SurveyQuestionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AddedBy")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Question")
                        .HasColumnType("TEXT");

                    b.Property<string>("SurveyId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("SurveyQuestionId");

                    b.HasIndex("SurveyId");

                    b.ToTable("SurveyQuestions");
                });

            modelBuilder.Entity("Groundforce.Services.Models.SurveyType", b =>
                {
                    b.Property<string>("SurveyTypeId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AddedBy")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("SurveyTypeId");

                    b.ToTable("SurveyTypes");
                });

            modelBuilder.Entity("Groundforce.Services.Models.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ActualAmount")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("AddedBy")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("FieldAgentId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PaidAmount")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("PaidAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reference")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FieldAgentId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Groundforce.Services.Models.UserSurvey", b =>
                {
                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SurveyId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AddedBy")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("ApplicationUserId", "SurveyId");

                    b.HasIndex("SurveyId");

                    b.ToTable("UserSurveys");
                });

            modelBuilder.Entity("Groundforce.Services.Models.VerificationItem", b =>
                {
                    b.Property<string>("ItemId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApplicationUserId")
                        .IsRequired()
                        .HasColumnName("AddedBy")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(250);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(20);

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("ItemId");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("VerificationItems");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Groundforce.Services.Models.FieldAgent", b =>
                {
                    b.HasOne("Groundforce.Services.Models.ApplicationUser", "ApplicationUser")
                        .WithOne("FieldAgent")
                        .HasForeignKey("Groundforce.Services.Models.FieldAgent", "ApplicationUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Groundforce.Services.Models.Mission", b =>
                {
                    b.HasOne("Groundforce.Services.Models.FieldAgent", "FieldAgent")
                        .WithMany("Missions")
                        .HasForeignKey("FieldAgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Groundforce.Services.Models.VerificationItem", "VerificationItem")
                        .WithOne("Mission")
                        .HasForeignKey("Groundforce.Services.Models.Mission", "VerificationItemId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Groundforce.Services.Models.MissionVerified", b =>
                {
                    b.HasOne("Groundforce.Services.Models.BuildingType", "BuildingType")
                        .WithMany("MissionsVerified")
                        .HasForeignKey("BuildingTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Groundforce.Services.Models.Mission", "Mission")
                        .WithOne("MissionVerified")
                        .HasForeignKey("Groundforce.Services.Models.MissionVerified", "MissionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Groundforce.Services.Models.NotificationUser", b =>
                {
                    b.HasOne("Groundforce.Services.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("NotificationUsers")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Groundforce.Services.Models.Notification", "Notification")
                        .WithMany()
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Groundforce.Services.Models.PointAllocated", b =>
                {
                    b.HasOne("Groundforce.Services.Models.FieldAgent", "FieldAgent")
                        .WithMany("PointsAllocated")
                        .HasForeignKey("FieldAgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Groundforce.Services.Models.Point", "Point")
                        .WithMany("PointsAllocated")
                        .HasForeignKey("PointId");
                });

            modelBuilder.Entity("Groundforce.Services.Models.QuestionOption", b =>
                {
                    b.HasOne("Groundforce.Services.Models.SurveyQuestion", "SurveyQuestion")
                        .WithMany("QuestionOptions")
                        .HasForeignKey("SurveyQuestionId");
                });

            modelBuilder.Entity("Groundforce.Services.Models.Response", b =>
                {
                    b.HasOne("Groundforce.Services.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Groundforce.Services.Models.QuestionOption", "QuestionOption")
                        .WithMany()
                        .HasForeignKey("QuestionOptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Groundforce.Services.Models.Survey", "Survey")
                        .WithMany()
                        .HasForeignKey("SurveyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Groundforce.Services.Models.SurveyQuestion", "SurveyQuestion")
                        .WithMany()
                        .HasForeignKey("SurveyQuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Groundforce.Services.Models.UserSurvey", "UserSurvey")
                        .WithMany("Responses")
                        .HasForeignKey("UserSurveyApplicationUserId", "UserSurveySurveyId");
                });

            modelBuilder.Entity("Groundforce.Services.Models.Survey", b =>
                {
                    b.HasOne("Groundforce.Services.Models.SurveyType", "SurveyType")
                        .WithMany("Surveys")
                        .HasForeignKey("SurveyTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Groundforce.Services.Models.SurveyQuestion", b =>
                {
                    b.HasOne("Groundforce.Services.Models.Survey", "Survey")
                        .WithMany("Questions")
                        .HasForeignKey("SurveyId");
                });

            modelBuilder.Entity("Groundforce.Services.Models.Transaction", b =>
                {
                    b.HasOne("Groundforce.Services.Models.FieldAgent", "FieldAgent")
                        .WithMany("Transactions")
                        .HasForeignKey("FieldAgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Groundforce.Services.Models.UserSurvey", b =>
                {
                    b.HasOne("Groundforce.Services.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Groundforce.Services.Models.Survey", "Survey")
                        .WithMany("UserSurveys")
                        .HasForeignKey("SurveyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Groundforce.Services.Models.VerificationItem", b =>
                {
                    b.HasOne("Groundforce.Services.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("VerificationItems")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Groundforce.Services.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Groundforce.Services.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Groundforce.Services.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Groundforce.Services.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
