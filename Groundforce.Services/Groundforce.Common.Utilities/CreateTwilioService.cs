using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace Groundforce.Common.Utilities
{
    public class CreateTwilioService
    {
        private static IConfiguration _config;
        public static string ServiceSid { get; set; }

        public static void Init(IConfiguration config)
        {
            _config = config;
            ServiceSid = _config.GetSection("Twilio:ServiceSid").Value;
            string accountSid = _config.GetSection("Twilio:AccountSid").Value;
            string authToken = _config.GetSection("Twilio:AuthToken").Value;

            TwilioClient.Init(accountSid, authToken);
        }

        public static async Task<string> SendOTP(string phoneNumber)
        {
            var verificationResource = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: "sms",
                pathServiceSid: ServiceSid
            );
            return verificationResource.Status;                       
        }

        public static async Task<string> ConfirmOTP(string phoneNumber, string verifyCode)
        {
            VerificationCheckResource verificationCheck = await VerificationCheckResource.CreateAsync(
                        to: phoneNumber,
                        code: verifyCode,
                        pathServiceSid: ServiceSid
                        );
            return verificationCheck.Status;
        }
    }
}
