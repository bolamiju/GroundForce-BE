using System;
using System.Threading.Tasks;
using Groundforce.Services.DTOs;
using Groundforce.Common.Utilities;
using Groundforce.Services.Core;
using Groundforce.Services.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio.Exceptions;
using Microsoft.AspNetCore.Identity;
using Groundforce.Services.Models;
using System.Linq;

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
            //create instance of the phoneNumberService class
            var numberService = new PhoneNumberService(_ctx);
            PhoneNumberStatus phoneNumberStatus;
            try
            {
                //call the phone number check method
                phoneNumberStatus = await numberService.PhoneNumberCheck(model.PhoneNumber);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (phoneNumberStatus == PhoneNumberStatus.Blocked) return BadRequest("Number blocked");
            if (phoneNumberStatus == PhoneNumberStatus.InvalidRequest) return BadRequest();
            if (phoneNumberStatus == PhoneNumberStatus.Verified) return BadRequest("Number already registered");

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