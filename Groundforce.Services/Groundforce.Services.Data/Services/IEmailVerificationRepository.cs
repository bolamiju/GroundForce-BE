using Groundforce.Services.Models;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IEmailVerificationRepository
    {
        Task<bool> AddEmailVerification(EmailVerification model);
        Task<bool> DeleteEmailVerification(EmailVerification model);
        Task<EmailVerification> GetEmailVerificationById(string Id);
        Task<EmailVerification> GetEmailVerificationByEmail(string email);
        Task<bool> UpdateEmailVerification(EmailVerification model);
    }
}