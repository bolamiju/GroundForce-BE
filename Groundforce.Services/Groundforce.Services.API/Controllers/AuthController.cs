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
using System.Collections.Generic;

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
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAgentRepository _agentRepository;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger,
                                 SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager, AppDbContext ctx,
                                 IRequestRepository requestRepository, IAgentRepository agentRepository)
        {
            _config = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _ctx = ctx;
            _requestRepository = requestRepository;
            _userManager = userManager;
            _agentRepository = agentRepository;
        }


        //verify OTP
        [HttpPost("verify-phone")]
        public async Task<IActionResult> VerifyPhone([FromBody] PhoneNumberToVerifyDTO model)
        {          
            if(String.IsNullOrWhiteSpace(model.PhoneNumber)) return BadRequest(ResponseMessage.Message("Bad request", "Invalid request credential"));

            Request number = null;
            try
            {
                // fetch records related to phone number if it already exists in the table
                number = await _requestRepository.GetRequestByPhone(model.PhoneNumber);                             
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", "Could not access record related to phone number"));
            }

            try
            {
                if (number != null)
                {
                    // this part checks for the current status of the number on the request table
                    var lobbyService = new LobbyService(_ctx);
                    await lobbyService.CheckPhoneNumber(number);
                }
            }
            catch (Exception e)
            {
                return BadRequest(ResponseMessage.Message("Bad request", e.Message));
            }

            try
            {
                if (number != null)
                {
                    // this part only increases the count of the request attempts
                    number.RequestAttempt += 1;
                    await _requestRepository.UpdateRequest(number);
                }
                else
                {
                    // this part adds a number to request table if it has not been added before
                    string requestId = "";
                    Request result = null;
                    do
                    {
                        requestId = Guid.NewGuid().ToString();
                        result = await _requestRepository.GetRequestById(requestId);
                    } while (result != null);

                    //adds number to the database
                    await _ctx.AddAsync(new Request()
                    {
                        RequestId = requestId,
                        PhoneNumber = model.PhoneNumber,
                        RequestAttempt = 1
                    });
                    await _ctx.SaveChangesAsync();
                }
            }catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", "Error with database processing"));
            }

            try
            {
                CreateTwilioService.Init(_config);
                await CreateTwilioService.SendOTP(model.PhoneNumber);
                return Ok(ResponseMessage.Message("Success",null,"OTP sent!"));
            }
            catch (TwilioException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request","Failed to send OTP"));
            }
        }


        // confirm OTP
        [HttpPost("confirm-otp")]
        public async Task<IActionResult> ConfirmOTP([FromBody] OTPToConfirmDTO model)
        {
            if (String.IsNullOrWhiteSpace(model.PhoneNumber)) return BadRequest(ResponseMessage.Message("Bad request", "Invalid request credentials"));

            Request number = null;
            try
            {
                // fetch records related to phone number if it already exists in the table
                number = await _requestRepository.GetRequestByPhone(model.PhoneNumber);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", "Could not access record related to phone number"));
            }

            // if phone number has gone through verification
            if (number == null) return BadRequest(ResponseMessage.Message("Bad request", "Phone number has not gone through verification yet"));

            
            if (number != null)
            {
                try
                {
                    // this part checks for the current status of the number on the request table
                    var lobbyService = new LobbyService(_ctx);
                    await lobbyService.CheckPhoneNumber(number);
                }
                catch (Exception e)
                {
                    return BadRequest(ResponseMessage.Message("Bad request", e.Message));
                }
            }
            
            string status = "";
            try
            {
                CreateTwilioService.Init(_config);
                status = await CreateTwilioService.ConfirmOTP(model.PhoneNumber, model.VerifyCode);
            }
            catch (TwilioException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", "Failed to confirm OTP try requesting for a new OTP"));
            }

            try
            {
                if (status == PhoneNumberStatus.pending.ToString().ToLower())
                {
                    return BadRequest(ResponseMessage.Message("Bad request", "OTP does not match"));
                }

                number.Status = "confirmed";
                if (!await _requestRepository.UpdateRequest(number))
                    throw new Exception("Could not update request");
                return Ok(ResponseMessage.Message("Success", null, "OTP confirmed!"));

            }catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", "Error with database processing"));
            }

        }


        // register user
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAgent(UserToRegisterDTO model)
        {

            try
            {
                // ensure that number has gone through verification and confirmation
                var phoneNumberIsInRequestTable = await _requestRepository.GetRequestByPhone(model.PhoneNumber);
                if (phoneNumberIsInRequestTable == null)
                    return BadRequest(ResponseMessage.Message("Bad request", "Phone number has not gone through verification yet"));

                if (phoneNumberIsInRequestTable.Status == "pending")
                    return BadRequest(ResponseMessage.Message("Bad request", "Phone number has not been confirmed yet"));

            }catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", "Data processing error"));
            }

            ApplicationUser createdUser = null;
            try
            {
                // check if email aready exists
                var emailToAdd = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);
                if (emailToAdd != null)
                    return BadRequest(ResponseMessage.Message("Bad request", "Email already exist"));

                // check if number aready exists
                var numberToAdd = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == model.PhoneNumber);
                if (numberToAdd != null)
                    return BadRequest(ResponseMessage.Message("Bad request", "Phone number already exist"));

                if(model.Roles.Contains("admin") || model.Roles.Contains("client"))
                {
                    if (!User.Identity.IsAuthenticated)
                        return Unauthorized(ResponseMessage.Message("Unauthorized", "User must be signed-in, to register other users"));
                   
                    if (!User.IsInRole("Admin"))
                        return Unauthorized(ResponseMessage.Message("Unauthorized", "User must be an Admin to perform this task"));
                    
                }

                // create user
                AuthSupportService auth = new AuthSupportService(_userManager, _agentRepository);
                var result = await auth.CreateUser(model);

                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return BadRequest(ResponseMessage.Message("Bad request", ModelState));
                }

                // create agent
                if (model.Roles.Contains("agent"))
                {
                    var successResult = await auth.CreateFieldAgent(model, createdUser.Id);
                    if (!successResult) return BadRequest(ResponseMessage.Message("Bad request", "Could not create field agent profile"));
                }
               
            }catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", "Data processing error"));
            }

            return BadRequest(ResponseMessage.Message("Success", null , new { createdUser.Id }));

        }


        //User Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserToLoginDTO model)
        {
           
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ResponseMessage.Message("Bad request", ModelState));
            
                //get user by email
                var user = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);

                //Check if user exist
                if (user == null)
                {
                    return Unauthorized(ResponseMessage.Message("Unauthorized", "Invalid credentials"));
                }

                if (!user.IsActive)
                    return Unauthorized(ResponseMessage.Message("Unauthorized", "In-active account"));

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Pin, false, false);
                var userRoles = await _userManager.GetRolesAsync(user);
                if (result.Succeeded)
                {
                    var getToken = JwtTokenConfig.GetToken(user, _config, userRoles);
                    return Ok(ResponseMessage.Message("Success",null, new { token = getToken }));
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", "Data processing error"));
            }

            return Unauthorized(ResponseMessage.Message("Unauthorized", "Invalid credentials"));

        }


     
    }
}