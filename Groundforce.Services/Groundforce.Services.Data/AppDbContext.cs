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

        public DbSet<FieldAgent> FieldAgent { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AssignedAddresses> AssignedAddresses { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BuildingType> BuildingTypes { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<PointAllocated> PointAllocated { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
