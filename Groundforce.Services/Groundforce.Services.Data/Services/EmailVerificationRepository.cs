using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class EmailVerificationRepository : IEmailVerificationRepository
    {
        private readonly AppDbContext _ctx;

        public EmailVerificationRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<bool> AddEmailVerification(EmailVerification model)
        {
            await _ctx.EmailVerifications.AddAsync(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteEmailVerification(EmailVerification model)
        {
            _ctx.EmailVerifications.Remove(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<EmailVerification> GetEmailVerificationById(string Id)
        {
            return await _ctx.EmailVerifications.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<EmailVerification> GetEmailVerificationByEmail(string email)
        {
            return await _ctx.EmailVerifications.FirstOrDefaultAsync(x => x.EmailAddress == email);
        }

        public async Task<bool> UpdateEmailVerification(EmailVerification model)
        {
            _ctx.EmailVerifications.Update(model);
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}