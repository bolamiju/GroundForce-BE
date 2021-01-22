using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class PasswordVerificationRepository : IPasswordVerificationRepository
    {
        private readonly AppDbContext _ctx;
        public PasswordVerificationRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<bool> AddPasswordVerification(PasswordVerification model)
        {
            await _ctx.PasswordVerifications.AddAsync(model);
            return await _ctx.SaveChangesAsync() > 0 ;
        }

        public async Task<PasswordVerification> GetPasswordVerificationByEmail(string email)
        {
            return await _ctx.PasswordVerifications.FirstOrDefaultAsync(x => x.EmailAddress == email);
        }

        public async Task<PasswordVerification> GetPasswordVerificationByEmailAndToken(string email, string token)
        {
            return await _ctx.PasswordVerifications.FirstOrDefaultAsync(x => x.EmailAddress == email && x.Token == token);
        }

        public async Task<bool> UpdatePasswordVerification(PasswordVerification model)
        {
            _ctx.PasswordVerifications.Update(model);
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
