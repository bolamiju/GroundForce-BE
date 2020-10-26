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
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // private fields
        private readonly IConfiguration _config;
        private readonly AppDbContext _ctx;
        private readonly IRequestRepository _requestRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IBankRepository _bankRepository;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;


        public AuthController(IConfiguration configuration, ILogger<AuthController> logger,
                                 SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager, AppDbContext ctx,
                                 IRequestRepository requestRepository, IAgentRepository agentRepository,
                                 IBankRepository bankRepository)
        {
            _config = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _ctx = ctx;
            _requestRepository = requestRepository;
            _agentRepository = agentRepository;
            _bankRepository = bankRepository;
            _userManager = userManager;
        }

        //verify OTP
        [HttpPost("verification")]
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
                return BadRequest("Failed to update user verification request status");
            }

            if (phoneNumberStatus == PhoneNumberStatus.Blocked) return BadRequest("Number blocked");
            if (phoneNumberStatus == PhoneNumberStatus.InvalidRequest) return BadRequest();
            if (phoneNumberStatus == PhoneNumberStatus.Verified) return BadRequest("Number already registered");

            try
            {
                CreateTwilioService.Init(_config);
                await CreateTwilioService.SendOTP(model.PhoneNumber);
                return Ok("OTP sent!");
            }
            catch (TwilioException e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Failed to send OTP");
            }
        }

        //confirm OTP
        [HttpPost("confirmation")]
        public async Task<IActionResult> ConfirmOTP([FromBody] OTPToConfirmDTO model)
        {
            try
            {
                CreateTwilioService.Init(_config);
                await CreateTwilioService.ConfirmOTP(model.PhoneNumber, model.VerifyCode);
            }
            catch (TwilioException e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Failed to confirm OTP");
            }
            try
            {
                //get the phone number of the successfully registered user 
                var registeredUser = await _requestRepository.GetRequestByPhone(model.PhoneNumber);
                registeredUser.IsConfirmed = true;
                if(!await _requestRepository.UpdateRequest(registeredUser))
                  throw new Exception("Could not update request");

                return Ok("OTP confirmed!");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Request not confirmed!");
            }
        }


        //// register user
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserToRegisterDTO model)
        {
            // check if email aready exists
            var userToAdd = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);
            if (userToAdd != null)
                return BadRequest("Email already exist");

            var phoneNumberIsInRequestTable = await _requestRepository.GetRequestByPhone(model.PhoneNumber);
            if (phoneNumberIsInRequestTable == null)
                return BadRequest("Phone number has not gone through verification process");


            //Add new applicationUser
            var authSupportService = new AuthSupportService(_userManager, _agentRepository, _bankRepository);
            var result = await authSupportService.CreateAppUser(model);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return BadRequest(ModelState);
            }


            //Add field agent
            ApplicationUser createdUser = await _userManager.FindByEmailAsync(model.Email);
            bool isAgentCreated = false;
            if (createdUser != null)
            {
                try
                {
                    isAgentCreated = await authSupportService.CreateFieldAgent(createdUser.Id, model);
                }
                catch (Exception e)
                {
                    await _userManager.DeleteAsync(createdUser);
                    await _userManager.RemoveFromRoleAsync(createdUser, "Agent");
                    _logger.LogError(e.Message);
                    return BadRequest("Failed to add additional details");
                }
            }
            else { return BadRequest("User not created"); }


            // Add bank details
            bool isBankCreated = false;
            FieldAgent newCreatedAgent = null;
            if (isAgentCreated)
            {
                try
                {
                    newCreatedAgent = await _agentRepository.GetAgentById(createdUser.Id);
                    isBankCreated = await authSupportService.CreateBankDetails(newCreatedAgent.FieldAgentId, model);
                }
                catch (Exception e)
                {
                    await _userManager.DeleteAsync(createdUser);
                    await _userManager.RemoveFromRoleAsync(createdUser, "Agent");
                    await _agentRepository.DeleteAgent(newCreatedAgent);
                    _logger.LogError(e.Message);
                    return BadRequest("Failed to add bank details");
                }
            }
            else { return BadRequest("Field agent not created"); }

            return RedirectToAction("Get", "User", new { Id = createdUser.Id });
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
                    return BadRequest("Account does not exist");
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Pin, false, false);
                var userRoles = await _userManager.GetRolesAsync(user);
                if (result.Succeeded)
                {
                    var getToken = JwtTokenConfig.GetToken(user, _config, userRoles);
                    return Ok(getToken);
                }
                return Unauthorized("Invalid creadentials");

            }

            return BadRequest(model);
        }


        // forgot password route
        [HttpPatch("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO details)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.Users.SingleOrDefault(e => e.PhoneNumber == details.PhoneNumber);
                if (user == null) return NotFound();
                //generate token needed to reset password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var setNewPassword = await _userManager.ResetPasswordAsync(user, token, details.Pin);
                if (setNewPassword.Succeeded) return Ok("Password successfully updated");

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