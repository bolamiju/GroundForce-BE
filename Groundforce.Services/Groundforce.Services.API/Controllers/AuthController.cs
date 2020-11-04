using System;
using System.Threading.Tasks;
using Groundforce.Common.Utilities;
using Groundforce.Common.Utilities.Helpers;
using Groundforce.Services.Core;
using Groundforce.Services.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio.Exceptions;
using Microsoft.AspNetCore.Identity;
using Groundforce.Services.Models;
using Groundforce.Services.DTOs;
using Groundforce.Services.Data.Services;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // private fields
        private readonly IConfiguration _config;
        private readonly AppDbContext _ctx;
        private readonly IRequestRepository _requestRepository;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;


        public AuthController(IConfiguration configuration, ILogger<AuthController> logger,
                                 SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager, AppDbContext ctx,
                                 IRequestRepository requestRepository)
        {
            _config = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _ctx = ctx;
            _requestRepository = requestRepository;
            _userManager = userManager;
        }

        //verify OTP
        [HttpPost("verifyPhone")]
        public async Task<IActionResult> VerifyPhone([FromBody] PhoneNumberToVerifyDTO model)
        {
            PhoneNumberStatus phoneNumberStatus;
            try
            {
                //create instance of the phoneNumberService class
                var updateRequestStatus = new LobbyService(_ctx);
                
                // check for valid GUID
                Request result = null;
                string requestId = "";
                do
                {
                    requestId = Guid.NewGuid().ToString();
                    result = await _requestRepository.GetRequestById(requestId);
                } while (result != null);

                phoneNumberStatus = await updateRequestStatus.CheckPhoneNumber(model.PhoneNumber, requestId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Failed to update user verification request status"));
            }

            if (phoneNumberStatus == PhoneNumberStatus.Blocked) return BadRequest(ResponseMessage.Message("Number blocked"));
            if (phoneNumberStatus == PhoneNumberStatus.InvalidRequest) return BadRequest(ResponseMessage.Message("Invalid request"));
            if (phoneNumberStatus == PhoneNumberStatus.Verified) return BadRequest(ResponseMessage.Message("Number already verified"));

            try
            {
                CreateTwilioService.Init(_config);
                await CreateTwilioService.SendOTP(model.PhoneNumber);
                return Ok(ResponseMessage.Message("OTP sent!"));
            }
            catch (TwilioException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Failed to send OTP"));
            }
        }


        [HttpPost("confirmOTP")]
        public async Task<IActionResult> ConfirmOTP([FromBody] OTPToConfirmDTO model)
        {
            string status = "";
            try
            {
                CreateTwilioService.Init(_config);
                status = await CreateTwilioService.ConfirmOTP(model.PhoneNumber, model.VerifyCode);
            }
            catch (TwilioException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Failed to confirm OTP"));
            }
            try
            {
                var registeredUser = await _requestRepository.GetRequestByPhone(model.PhoneNumber);
                //get the phone number of the successfully registered user 
                if (status != PhoneNumberStatus.Approved.ToString().ToLower())
                {
                    registeredUser.IsConfirmed = false;
                    return BadRequest(ResponseMessage.Message("OTP does not match"));
                }

                registeredUser.IsConfirmed = true;
                if (!await _requestRepository.UpdateRequest(registeredUser))
                    throw new Exception("Could not update request");


                return Ok(ResponseMessage.Message("OTP confirmed!"));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Request not confirmed"));
            }
        }


        //User Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserToLoginDTO model)
        {
            if (ModelState.IsValid)
            {
                //get user by email
                var user = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);

                //Check if user exist
                if (user == null)
                {
                    return NotFound(ResponseMessage.Message("User not found, ensure credentials are entered correctly."));
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Pin, false, false);
                var userRoles = await _userManager.GetRolesAsync(user);
                if (result.Succeeded)
                {
                    var getToken = JwtTokenConfig.GetToken(user, _config, userRoles);
                    return Ok(new { token = getToken});
                }
                return Unauthorized(ResponseMessage.Message("Invalid credentials"));

            }

            return BadRequest(model);
        }


        // forgot password route
        [HttpPost("forgotPassword/verifyPhone")]
        public async Task<IActionResult> ForgotPasswordVerify([FromBody] PhoneNumberToVerifyDTO details)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CreateTwilioService.Init(_config);
                    await CreateTwilioService.SendOTP(details.PhoneNumber);
                    return Ok(ResponseMessage.Message("OTP sent!"));
                }
                catch (TwilioException e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Failed to send OTP"));
                }
            }
            return BadRequest();
        }

        // forgot password route
        [HttpPatch("forgotPassword/resetPin")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO details)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.Users.SingleOrDefault(e => e.PhoneNumber == details.PhoneNumber);
                if (user == null) return NotFound(ResponseMessage.Message($"User with phone number: {details.PhoneNumber}, is not found"));

                string status = "";
                try
                {
                    CreateTwilioService.Init(_config);
                    status = await CreateTwilioService.ConfirmOTP(details.PhoneNumber, details.OTPCode);
                }
                catch (TwilioException e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("OTP confirmation failed"));
                }

                if (status != PhoneNumberStatus.Approved.ToString().ToLower())
                    return BadRequest(ResponseMessage.Message("OTP does not match"));

                //generate token needed to reset password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var setNewPassword = await _userManager.ResetPasswordAsync(user, token, details.NewPin);
                if (setNewPassword.Succeeded) return Ok(ResponseMessage.Message("Password successfully updated"));

                // if passwordset is unsuccessful add errors to model error
                foreach (var error in setNewPassword.Errors)
                {
                    ModelState.AddModelError("Error", error.Description);
                }
            }
            return BadRequest();
        }

    }
}