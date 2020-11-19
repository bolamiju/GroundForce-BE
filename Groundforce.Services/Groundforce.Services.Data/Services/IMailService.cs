using Groundforce.Services.Models;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IMailService
    {
        Task SendMailAsync(MailRequest mailRequest);
        Task  SendForgotPasswordEmailAsync(ForgotPasswordRequest request);
        Task SendWelcomeRequestAsync(WelcomeRequest request);
    }
}
