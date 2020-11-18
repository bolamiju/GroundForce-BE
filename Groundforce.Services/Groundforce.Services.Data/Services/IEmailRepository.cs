using Groundforce.Services.Models;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IEmailRepository
    {
        Task<EmailVerification> FindByEmailAddress(string emailAddress);
        Task<bool> UpdateEmailVerificationStatus(EmailVerification model);
    }
}