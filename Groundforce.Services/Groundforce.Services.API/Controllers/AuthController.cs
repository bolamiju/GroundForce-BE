using System;
using System.Threading.Tasks;
using Groundforce.Services.DTOs;
using Groundforce.Common.Utilities;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _config = configuration;
            _userManager = userManager;
        }

        // verify OTP
        [HttpPost("verification")]
        public async Task<IActionResult> Verification([FromBody] SendOTPDTOs model)
        {
            var user = _userManager.Users.SingleOrDefault(user => user.PhoneNumber == model.PhoneNumber);
            if (user == null) return NotFound();

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