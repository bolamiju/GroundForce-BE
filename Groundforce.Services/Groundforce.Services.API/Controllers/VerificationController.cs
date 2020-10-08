﻿using System;
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
    public class VerificationController : ControllerBase
    {
        private readonly TwilioService _service;
        private IConfiguration _config;

        public VerificationController(IConfiguration config)
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


    }


}
