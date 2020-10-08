using Groundforce.Services.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

        public DbSet<FieldAgent> FieldAgents { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<AssignedAddresses> AssignedAddresses { get; set; }
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
            builder.Entity<AssignedAddresses>()
                       .HasOne<FieldAgent>(e => e.FieldAgent)
                       .WithMany(e => e.AssignedAddresses)
                       .HasForeignKey(e => e.FieldAgentId)
                       .OnDelete(DeleteBehavior.Cascade);

            // Configure FieldAgent & AssignedAddresses entity
            builder.Entity<AssignedAddresses>()
                       .HasOne<Admin>(e => e.Admin)
                       .WithMany(e => e.AssignedAddresses)
                       .HasForeignKey(e => e.AdminId)
                       .OnDelete(DeleteBehavior.NoAction);

            // Configure FieldAgent & AssignedAddresses entity
            builder.Entity<AssignedAddresses>()
                       .HasOne<BuildingType>(e => e.BuildingType)
                       .WithMany(e => e.AssignedAddresses)
                       .HasForeignKey(e => e.BuildingTypeId)
                       .OnDelete(DeleteBehavior.Cascade);

            // Configure FieldAgent & AssignedAddresses entity
            builder.Entity<AssignedAddresses>()
                       .HasOne<Address>(e => e.Address)
                       .WithOne(e => e.AssignedAddresses)
                       .HasForeignKey<Address>(e => e.AddressId)
                       .OnDelete(DeleteBehavior.NoAction);

            // Configure Points & PointAllocated entity
            builder.Entity<PointAllocated>()
                       .HasOne<FieldAgent>(e => e.FieldAgent)
                       .WithMany(e => e.PointsAllocated)
                       .HasForeignKey(e => e.FieldAgentId)
                       .OnDelete(DeleteBehavior.Cascade);


            // Configure FieldAgent & PointAllocated entity
            builder.Entity<PointAllocated>()
                       .HasOne<Admin>(e => e.Admin)
                       .WithMany(e => e.PointsAllocated)
                       .HasForeignKey(e => e.AdminId)
                       .OnDelete(DeleteBehavior.NoAction);


            // Configure FieldAgent & PointAllocated entity
            builder.Entity<PointAllocated>()
                       .HasOne<Point>(e => e.Point)
                       .WithMany(e => e.PointsAllocated)
                       .HasForeignKey(e => e.PointsId)
                       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
