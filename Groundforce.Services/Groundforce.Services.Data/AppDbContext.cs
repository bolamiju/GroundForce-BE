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
        public DbSet<UserActivity> UserActivity { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<MissionVerified> MissionsVerified { get; set; }
        public DbSet<BuildingType> BuildingTypes { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<PointAllocated> PointAllocated { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Request> Request { get; set; }
      
    }
}