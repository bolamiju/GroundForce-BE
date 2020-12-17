using Groundforce.Services.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Groundforce.Services.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<FieldAgent> FieldAgents { get; set; }
        public DbSet<VerificationItem> VerificationItems { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<MissionVerified> MissionsVerified { get; set; }
        public DbSet<BuildingType> BuildingTypes { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<PointAllocated> PointAllocated { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Request> Request { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyType> SurveyTypes { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<UserSurvey> UserSurveys { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<NotificationUser> NotificationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserSurvey>().HasKey(fas => new { fas.ApplicationUserId, fas.SurveyId });

            // Configure Mission & VerificationItem entity
            builder.Entity<Mission>()
                      .HasOne<VerificationItem>(e => e.VerificationItem)
                      .WithOne(e => e.Mission)
                      .HasForeignKey<Mission>(e => e.VerificationItemId)
                      .OnDelete(DeleteBehavior.NoAction);
            // Configure VerificationItem & Mission entity
            builder.Entity<VerificationItem>()
                       .HasOne<Mission>(e => e.Mission)
                       .WithOne(e => e.VerificationItem)
                       .HasForeignKey<Mission>(e => e.VerificationItemId)
                       .OnDelete(DeleteBehavior.NoAction);

            // Configure Mission & MissionVerified entity
            builder.Entity<Mission>()
                      .HasOne<MissionVerified>(e => e.MissionVerified)
                      .WithOne(e => e.Mission)
                      .HasForeignKey<MissionVerified>(e => e.MissionId)
                      .OnDelete(DeleteBehavior.NoAction);
            // Configure MissionVerified & Mission entity
            builder.Entity<MissionVerified>()
                       .HasOne<Mission>(e => e.Mission)
                       .WithOne(e => e.MissionVerified)
                       .HasForeignKey<MissionVerified>(e => e.MissionId)
                       .OnDelete(DeleteBehavior.NoAction);


            // Configure ApplicaitonUser & FieldAgent entity
            builder.Entity<ApplicationUser>()
                      .HasOne<FieldAgent>(e => e.FieldAgent)
                      .WithOne(e => e.ApplicationUser)
                      .HasForeignKey<FieldAgent>(e => e.ApplicationUserId)
                      .OnDelete(DeleteBehavior.NoAction);
            // Configure FieldAgent & ApplicationUser entity
            builder.Entity<FieldAgent>()
                       .HasOne<ApplicationUser>(e => e.ApplicationUser)
                       .WithOne(e => e.FieldAgent)
                       .HasForeignKey<FieldAgent>(e => e.ApplicationUserId)
                       .OnDelete(DeleteBehavior.NoAction);
        }
    }
}