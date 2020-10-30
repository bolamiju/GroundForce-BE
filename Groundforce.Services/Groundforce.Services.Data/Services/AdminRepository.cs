using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _ctx;

        public AdminRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> AddAdmin(Admin model)
        {
            await _ctx.Admins.AddAsync(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAdmin(Admin model)
        {
            _ctx.Admins.Remove(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<Admin> GetAdminById(string Id)
        {
            return await _ctx.Admins.FirstOrDefaultAsync(x => x.ApplicationUserId == Id || x.AdminId == Id);
        }
    }
}
