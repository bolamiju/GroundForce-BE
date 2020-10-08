using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Twilio;
using Twilio.Rest.Verify.V2;
using Twilio.Rest.Verify.V2.Service;

namespace Groundforce.Common.Utilities.Util
{
    /// <summary>
    /// This Creates the Twilio Service
    /// </summary>
    public class TwilioService
    {
        /// <summary>
        /// Global varaible
        /// </summary>
        private readonly IConfiguration _config;
        /// <summary>
        /// Constructor for the class
        /// </summary>
        /// <param name="config"></param>
        public TwilioService(IConfiguration config)
        {
            _config = config;
        }
        /// <summary>
        /// Creates my twilio service
        /// </summary>
        public void CreateTwilioService()
        {
            string auth_token = _config.GetSection("AuthyApiKey:TWILIO_AUTH_TOKEN").Value;
            string sid = _config.GetSection("AuthyApiKey:TWILIO_ACCOUNT_SID").Value;

            TwilioClient.Init(sid, auth_token);
        }
        /// <summary>
        /// Sends OTP code to phonemunber via twilio
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>The verification status as string</returns>
        public string SentOtp(string phoneNumber)
        {
            CreateTwilioService();

            var verification = VerificationResource.Create(
                _config.GetSection("AuthyApiKey:SERVICE_SID").Value,
                phoneNumber,
                "sms"
            );

            return verification.Status;
        }   
        

         public VerificationCheckResource VerifyPhoneNumberWithToken(string phoneNumber, string token)
        {
            CreateTwilioService();

            // verify number 
            var verificationCheck = VerificationCheckResource.Create(
                to: phoneNumber,
                code: token,
                pathServiceSid: _config.GetSection("AuthyApiKey:SERVICE_SID").Value
            );

            // return result 
            return verificationCheck;

        }
    }
}
