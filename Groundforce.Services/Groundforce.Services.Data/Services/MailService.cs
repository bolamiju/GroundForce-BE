using Groundforce.Services.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class MailService : IMailService
    {
        private readonly SendGridSettings _sendGridSettings;
        public MailService(IOptions<SendGridSettings> sendGridSettings)
        {
            _sendGridSettings = sendGridSettings.Value;
        }

        public async Task SendMailAsync(MailRequest request)
        {
            StreamReader str = new StreamReader(@"MailSender.html");
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[GroundForceUrl]", request.GroundForceUrl).Replace("[GroundForceLogo]", _sendGridSettings.GroundForceLogo)
                .Replace("[MainHeader]", request.MainHeader).Replace("[SubHeader]", request.SubHeader).Replace("[Content]", request.Content);
            if (request.IsHidden)
            {
                MailText = MailText.Replace("[Hidden]", "hidden").Replace("[ButtonName]", "");
            }
            else
            {
                MailText = MailText.Replace("[ButtonName]", request.ButtonName).Replace("[Link]", request.Link).Replace("[Hidden]", "show");
            }

            var apiKey = _sendGridSettings.APIKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_sendGridSettings.Mail, _sendGridSettings.DisplayName);
            var subject = _sendGridSettings.DisplayName;
            var to = new EmailAddress(request.ToEmail);
            var plainTextContent = MailText;
            var htmlContent = MailText;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode.ToString().ToLower() == "accepted")
            {
                return;
            }
            throw new Exception();
        }
    }
}
