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
    //class to contain the twilio services
    public class TwilioService
    {
        private readonly IConfiguration _configuration;

        public TwilioService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        //method to send otp to the phoneNumber collected 
        public async Task<string> SendOtp(string phoneNumber)
        {
            //get the AccountSid, AuthToken, Sid from the appsettings file
            string accountSid = _configuration.GetSection("AppSettings:AccountSid").Value;
            string authToken = _configuration.GetSection("AppSettings:AuthToken").Value;
            string sid = _configuration.GetSection("AppSettings:ServiceSID").Value;

            //set the twilio client username and password
                TwilioClient.Init(accountSid, authToken);
            //call the method to send the OTP
                var verification = await VerificationResource.CreateAsync(
                    to: phoneNumber,
                    channel: "sms",
                    pathServiceSid: sid
                );

                return verification.Status;
        }
        /** Method to confirm otp and the number that received the otp **/
        public async Task<string> ConfirmOtp(string phoneNumber, string verifyCode)
        {
            // get the AccountSid, AuthToken, Sid from the appsettings file
            string accountSid = _configuration.GetSection("AppSettings:AccountSid").Value;
            string authToken = _configuration.GetSection("AppSettings:AuthToken").Value;
            string sid = _configuration.GetSection("AppSettings:ServiceSID").Value;

            // Initializes Twillo client with the username and password
            TwilioClient.Init(accountSid, authToken);
            // Calls the method and checks the verification resource for confirmation
            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: verifyCode,
                pathServiceSid: sid
            );
            // Returns either pending or approved as a response
            return verificationCheck.Status;

        }
    }
}
