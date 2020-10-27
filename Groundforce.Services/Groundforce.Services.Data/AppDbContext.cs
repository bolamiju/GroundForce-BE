using Groundforce.Services.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Groundforce.Services.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<FieldAgent> FieldAgents { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<VerificationItem> VerificationItems { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BuildingType> BuildingTypes { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<PointAllocated> PointAllocated { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Request> Request { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Configure FieldAgent & AssignedAddresses entity
            builder.Entity<Mission>()
                       .HasOne<FieldAgent>(e => e.FieldAgent)
                       .WithMany(e => e.Missions)
                       .HasForeignKey(e => e.FieldAgentId)
                       .OnDelete(DeleteBehavior.Cascade);
            // Configure Mission & Admin entity
            builder.Entity<Mission>()
                       .HasOne<Admin>(e => e.Admin)
                       .WithMany(e => e.Missions)
                       .HasForeignKey(e => e.AdminId)
                       .OnDelete(DeleteBehavior.NoAction);
            // Configure Mission & BuildingType entity
            builder.Entity<Mission>()
                       .HasOne<BuildingType>(e => e.BuildingType)
                       .WithMany(e => e.Missions)
                       .HasForeignKey(e => e.BuildingTypeId)
                       .OnDelete(DeleteBehavior.Cascade);
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
            // Configure PointAllocated & FieldAgent entity
            builder.Entity<PointAllocated>()
                       .HasOne<FieldAgent>(e => e.FieldAgent)
                       .WithMany(e => e.PointsAllocated)
                       .HasForeignKey(e => e.FieldAgentId)
                       .OnDelete(DeleteBehavior.Cascade);
            // Configure PointAllocated & Admin entity
            builder.Entity<PointAllocated>()
                       .HasOne<Admin>(e => e.Admin)
                       .WithMany(e => e.PointsAllocated)
                       .HasForeignKey(e => e.AdminId)
                       .OnDelete(DeleteBehavior.NoAction);
            // Configure PointAllocated & Point entity
            builder.Entity<PointAllocated>()
                       .HasOne<Point>(e => e.Point)
                       .WithMany(e => e.PointsAllocated)
                       .HasForeignKey(e => e.PointsId)
                       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}