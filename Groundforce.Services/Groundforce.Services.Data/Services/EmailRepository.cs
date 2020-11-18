using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class EmailRepository : IEmailRepository
    {
        private readonly AppDbContext _ctx;

        public EmailRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<EmailVerification> FindByEmailAddress(string email)
        {
            return await _ctx.EmailVerifications.FirstOrDefaultAsync(mail => mail.EmailAddress == email);
        }

        public async Task<bool> UpdateEmailVerificationStatus(EmailVerification model)
        {
            _ctx.EmailVerifications.Update(model);
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}