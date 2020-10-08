using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Common.Utilities.Enums;
using Groundforce.Common.Utilities.Util;
using Groundforce.Services.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Verify.V2;
using Twilio.Rest.Verify.V2.Service;

namespace GroundforceModel.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class PhoneVerificationController : ControllerBase
    {
        /// <summary>
        /// Global variable
        /// </summary>
        private readonly IConfiguration _config;
        private readonly TwilioService _twilio;

        public PhoneVerificationController(IConfiguration config)
        {
            _config = config;
            _twilio = new TwilioService(_config);
        }
        /// <summary>
        /// Sends dotp to phone number
        /// </summary>
        /// <param name="client"></param>
        /// <returns>The status code</returns>
        [HttpPost("verification")]
        public IActionResult SendOTP([FromBody] PhoneNumberReceived client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Status.BadRequest);
            }
            
            if (_twilio.SentOtp(client.PhoneNumber) == Enum.GetName(typeof(TwilioStatus), TwilioStatus.pending))
            {
                return Ok((int)Status.Success);
            }

            return StatusCode((int)Status.InternalServerError);
        }

        /// <summary>
        /// get token and verify
        /// </summary>
        /// <param name="detail"></param>
        /// <returns>The status code </returns>
        [HttpPost("verify")]
        public IActionResult Verify([FromBody] TokenDetail detail)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest((int)Status.BadRequest);
            }

            // verify number 
            var verificationResult = _twilio.VerifyPhoneNumberWithToken(detail.PhoneNumber, detail.Token);
            // check if approved 
            if (verificationResult.Status == Enum.GetName(typeof(TwilioStatus), TwilioStatus.approved))
            {
                return Ok((int)Status.Success);
            }

            // check pending
            if (verificationResult.Status == Enum.GetName(typeof(TwilioStatus), TwilioStatus.pending))
            {
                return BadRequest((int)Status.BadRequest);
            }

            // return code 
            return StatusCode((int)Status.InternalServerError);

        }
    }
}
