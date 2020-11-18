using Groundforce.Services.Models;
using System.Threading.Tasks;

namespace Groundforce.Services.Core
{
    public interface IMailService
    {
        Task SendMailAsync(MailRequest mailRequest);
        Task  SendForgotPasswordEmailAsync(ForgotPasswordRequest request);
    }
}
