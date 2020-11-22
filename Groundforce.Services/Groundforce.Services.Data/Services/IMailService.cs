using Groundforce.Services.Models;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IMailService
    {
        Task SendMailAsync(MailRequest mailRequest);
    }
}
