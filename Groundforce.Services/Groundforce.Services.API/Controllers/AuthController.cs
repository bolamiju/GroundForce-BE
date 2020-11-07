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
using Microsoft.EntityFrameworkCore;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // private fields
        private readonly IConfiguration _config;
        private readonly AppDbContext _ctx;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBankRepository _bankRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IAgentRepository _agentRepository;


        public AuthController(IConfiguration configuration, ILogger<AuthController> logger,
                                 SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager, AppDbContext ctx,
                                 IRequestRepository requestRepository, IBankRepository bankRepository, 
                                 IAgentRepository agentRepository)
        {
            _config = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _ctx = ctx;
            _requestRepository = requestRepository;
            _userManager = userManager;
            _bankRepository = bankRepository;
            _agentRepository = agentRepository;
        }

        //verify OTP
        [HttpPost("verify-phone")]
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


        [HttpPost("confirm-otp")]
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

        //// register agent
        [HttpPost("register/agent")]
        public async Task<IActionResult> RegisterAgent(UserToRegisterDTO model)
        {
            // ensure that number has gone through verification and confirmation
            var phoneNumberIsInRequestTable = await _requestRepository.GetRequestByPhone(model.PhoneNumber);
            if (phoneNumberIsInRequestTable == null)
                return BadRequest(ResponseMessage.Message("Phone number has not been verified yet"));

            if (!phoneNumberIsInRequestTable.IsConfirmed)
                return BadRequest(ResponseMessage.Message("Phone number has not been confrimed yet"));


            // check if email aready exists
            var emailToAdd = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);
            if (emailToAdd != null)
                return BadRequest(ResponseMessage.Message("Email already exist"));

            // check if number aready exists
            var numberToAdd = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == model.PhoneNumber);
            if (numberToAdd != null)
                return BadRequest(ResponseMessage.Message("Phone number already exist"));


            //Add new applicationUser
            var userModel = new UserWithoutDetailsDTO
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DOB = model.DOB,
                LGA = model.LGA,
                PhoneNumber = model.PhoneNumber,
                PlaceOfBirth = model.PlaceOfBirth,
                State = model.State,
                Gender = model.Gender,
                HomeAddress = model.HomeAddress
            };

            var authSupportService = new AuthSupportService(_userManager, _agentRepository, _bankRepository);
            var result = await authSupportService.CreateAppUser(userModel, "Agent");
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
            if (createdUser == null)
                return BadRequest(ResponseMessage.Message("Failed to create identity user"));

            try
            {
                isAgentCreated = await authSupportService.CreateFieldAgent(createdUser.Id, model);
            }
            catch (Exception e)
            {
                await _userManager.DeleteAsync(createdUser);
                await _userManager.RemoveFromRoleAsync(createdUser, "Agent");
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Failed to add additional details"));
            }


            // Add bank details
            bool isBankCreated = false;
            FieldAgent newCreatedAgent = null;
            if (!isAgentCreated)
                return BadRequest(("Failed to create user"));

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
                return BadRequest(ResponseMessage.Message("Failed to add bank details"));
            }


            return Ok(ResponseMessage.Message("Picture upload was successful!", new { Message = "Account registered successfully", createdUser.Id }));
        }

        //// register agent location
        [HttpPatch("{id}/register-location")]
        public async Task<IActionResult> AddUserLocation(UserLocationDTO model, string id)
        {
            // ensure user can be found using id provided
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(ResponseMessage.Message($"User with id: {id} not found"));

            // ensure the state of the model is valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            FieldAgent agent = null;
            try
            {
                // get field agent with same id as user
                agent = await _agentRepository.GetAgentById(user.Id);
                if (agent == null)
                    return BadRequest(ResponseMessage.Message($"Could not find field agent related to user with id {user.Id}"));
            }catch(Exception e)
            {
                return BadRequest(ResponseMessage.Message(e.Message));
            }

            // update agent 
            agent.Longitude = model.Longitude;
            agent.Latitude = model.Latitude;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return BadRequest(ModelState);
            }
            
            return Ok(ResponseMessage.Message("User location updated!"));
        }

        //// register agent pin
        [HttpPost("{id}/register-pin")]
        public async Task<IActionResult> AddUserLocation(UserPinDTO model, string id)
        {
            // ensure user can be found using id provided
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(ResponseMessage.Message($"User with id: {id} not found"));

            // ensure the state of the model is valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            FieldAgent agent = null;
            try
            {
                // get field agent with same id as user
                agent = await _agentRepository.GetAgentById(user.Id);
                if (agent == null)
                    return BadRequest(ResponseMessage.Message($"Could not find field agent related to user with id {user.Id}"));
            }
            catch (Exception e)
            {
                return BadRequest(ResponseMessage.Message(e.Message));
            }

            // update agent 
            agent.Longitude = model.PIN;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok(ResponseMessage.Message("User pin updated!"));
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
        [HttpPost("forgot-pin/verify-phone")]
        public async Task<IActionResult> ForgotPasswordVerify([FromBody] PhoneNumberToVerifyDTO details)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.Users.SingleOrDefault(e => e.PhoneNumber == details.PhoneNumber);
                if (user == null) return NotFound(ResponseMessage.Message($"User with phone number: {details.PhoneNumber}, is not found"));

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
        [HttpPatch("forgot-pin/reset-pin")]
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