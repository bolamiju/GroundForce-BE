using System;
using System.Threading.Tasks;
using Groundforce.Services.DTOs;
using Groundforce.Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio.Exceptions;
using Groundforce.Services.Core;
using Groundforce.Services.Data;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // private fields
        private readonly IConfiguration _config;
        private readonly AppDbContext _ctx;


        public AuthController(IConfiguration configuration, AppDbContext ctx)
        {
            _config = configuration;
            _ctx = ctx;
    }
		
		// verify OTP
        [HttpPost("verification")]
        public async Task<IActionResult> Verification([FromBody] SendOTPDTOs model)
        {
            // check if number in database

            var numberStatus = new PhoneNumberStatus();

            try
            {
                numberStatus = await PhoneNumberRequest.CheckPhoneNumber(model.PhoneNumber, _ctx);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            // Execution of response from database checking
            if(numberStatus == PhoneNumberStatus.Verified)
            {
                return BadRequest("Number already registered");
            }
            else if(numberStatus == PhoneNumberStatus.Blocked)
            {
                return BadRequest("Contact Admin");
            }
            else
            {
                try
                {
                    CreateTwilioService.Init(_config);
                    await CreateTwilioService.SendOTP(model.PhoneNumber);
                    return Ok();
                }
                catch (TwilioException e)
                {
                    return BadRequest(e.Message);
                }
            }            
        }

        //confirm OTP
        [HttpPost("confirmation")]
        public async Task<IActionResult> Confirmation([FromBody] ConfirmationDTO model)
        {

            try
            {
                CreateTwilioService.Init(_config);
                await CreateTwilioService.ConfirmOTP(model.PhoneNumber, model.VerifyCode);
                return Ok();
            }
            catch (TwilioException e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
