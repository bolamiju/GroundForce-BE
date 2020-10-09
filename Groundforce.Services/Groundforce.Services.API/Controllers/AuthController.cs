using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Common.Utilities.Util;
using Groundforce.Services.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Twilio.Exceptions;


namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TwilioService _service;
        private IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _service = new TwilioService(config);
        }

        [HttpPost("verification")]
        public async Task<IActionResult> VerifyToken(SendOtpDto model)
        {
            if (!ModelState.IsValid) return BadRequest();

            try
            {
                //call the SendOTP method in the twilioService class
                var result = await _service.SendOtp(model.PhoneNumber);
                if (result == "pending")
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (TwilioException e)
            {
                return BadRequest(e.Message);
            }

        }
        [HttpPost("confirmation")]
        public async Task<IActionResult> ConfirmToken(ConfirmOtpDto model)
        {
            if (!ModelState.IsValid) return BadRequest();

            try
            {   // Calls the confirmOtp method in the twilloService class
                var response = await _service.ConfirmOtp(model.PhoneNumber, model.VerifyCode);

                if (response == "approved")
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            catch (TwilioException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
