using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Services.DTOs;
using Groundforce.Common.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _config;

        public AuthController(IConfiguration configuration)
        {
            _config = configuration;
        }

        // POST: api/<AuthController>/verification
        [HttpPost("verification")]
        public async Task<IActionResult> Verification([FromBody] SendOTPDTOs model)
        {
            CreateTwilioService.Init(_config);
            var status = await CreateTwilioService.SendOTP(model.PhoneNumber);

            if (status == Enum.GetName(typeof(TwilioStatus), TwilioStatus.pending))
            {
                return Ok();
            }

            return BadRequest();
        }

        //confirm OTP
        // api/v1/confirmation
        [HttpPost("confirmation")]
        public async Task<IActionResult> Confirmation([FromBody] ConfirmationDTO model)
        {
            string response = await CreateTwilioService.ConfirmOTP(model.PhoneNumber, model.VerifyCode);

            if (response.Equals("approved"))
            {
                return Ok();
            }
            else
            {
                return BadRequest(response);
            }
        }
    }
}
