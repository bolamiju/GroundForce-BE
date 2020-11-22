using Groundforce.Services.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendMailAsync(MailRequest request)
        {
            string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\MailSender.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[GroundForceUrl]", request.GroundForceUrl).Replace("[GroundForceLogo]", _mailSettings.GroundForceLogo)
                .Replace("[MainHeader]", request.MainHeader).Replace("[SubHeader]", request.SubHeader).Replace("[Content]", request.Content);
            if (request.IsHidden)
            {
                MailText = MailText.Replace("[Hidden]", "hidden").Replace("[ButtonName]", "");
            }
            else
            {
                MailText = MailText.Replace("[ButtonName]", request.ButtonName).Replace("[Link]", request.Link).Replace("[Hidden]", "show");
            }
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = $"Ground Force";
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
