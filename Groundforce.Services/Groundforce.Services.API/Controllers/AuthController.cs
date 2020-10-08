using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Services.DTOs.DTOs;
using Groundforce.Common.Utilities.Util;
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
    }
}
