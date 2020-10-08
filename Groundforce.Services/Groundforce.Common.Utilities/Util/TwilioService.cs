using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Types;

namespace Groundforce.Common.Utilities.Util
{
    public class TwilioService
    {
        private readonly IConfiguration _configuration;

        public TwilioService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> SendOtp(string phoneNumber)
        {
            string accountSid = _configuration.GetSection("AppSettings:AccountSid").Value;
            string authToken = _configuration.GetSection("AppSettings:AuthToken").Value;
            string sid = _configuration.GetSection("AppSettings:ServiceSID").Value;


           
                TwilioClient.Init(accountSid, authToken);

                var verification = await VerificationResource.CreateAsync(
                    to: phoneNumber,
                    channel: "sms",
                    pathServiceSid: sid
                );

                return verification.Status;


        }



    }
}
