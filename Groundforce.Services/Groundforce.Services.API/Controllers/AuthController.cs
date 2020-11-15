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
        [HttpPost("verify-phone")]
        public async Task<IActionResult> VerifyPhone([FromBody] PhoneNumberToVerifyDTO model)
        {          
            if(String.IsNullOrWhiteSpace(model.PhoneNumber)) return BadRequest(ResponseMessage.Message("Bad request", "Invalid request credentials"));

            Request number = null;
            try
            {
                // fetch records related to phone number if it already exists in the table
                number = await _requestRepository.GetRequestByPhone(model.PhoneNumber);                             
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Error processing data", "Could not access record related to phone number"));
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
                return BadRequest(ResponseMessage.Message("Error processing data", "Could not access record related to phone number"));
            }

            // if phone number has gone through verification
            if (number == null) return BadRequest(ResponseMessage.Message("Phone number has not gone through verification yet"));

            
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


        //// register user
        //[HttpPost("register")]
        //public async Task<IActionResult> RegisterAgent(UserToRegisterDTO model)
        //{

        //    // ensure that number has gone through verification and confirmation
        //    var phoneNumberIsInRequestTable = await _requestRepository.GetRequestByPhone(model.PhoneNumber);
        //    if (phoneNumberIsInRequestTable == null)
        //        return BadRequest(ResponseMessage.Message("Phone number has not been verified yet"));

        //    if (phoneNumberIsInRequestTable.Status == "pending")
        //        return BadRequest(ResponseMessage.Message("Phone number has not been confirmed yet"));


        //    var regParams = new Dictionary<string, string>();
        //    regParams.Add("Gender", model.Gender);
        //    regParams.Add("Religion", model.Religion);
        //    regParams.Add("State", model.State);
        //    regParams.Add("Place of birth", model.PlaceOfBirth);
        //    regParams.Add("LGA", model.LGA);
        //    regParams.Add("Bank", model.BankName);

        //    string output = InputValidator.WordInputValidator(regParams);

        //    if (output.Length > 0)
        //    {
        //        return BadRequest(ResponseMessage.Message("Invalid input: " + output));
        //    }

        //    response = InputValidator.AccountNumberValidator(model.AccountNumber);
        //    if (!response)
        //    {
        //        return BadRequest(ResponseMessage.Message("Account number must be 10 digits"));
        //    }

        //    response = InputValidator.NUBANAccountValidator(model.BankName, model.AccountNumber);
        //    if (!response)
        //        return BadRequest(ResponseMessage.Message("Account number for the Bank is invalid"));


        //    // check if email aready exists
        //    var emailToAdd = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);
        //    if (emailToAdd != null)
        //        return BadRequest(ResponseMessage.Message("Email already exist"));

        //    // check if number aready exists
        //    var numberToAdd = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == model.PhoneNumber);
        //    if (numberToAdd != null)
        //        return BadRequest(ResponseMessage.Message("Phone number already exist"));

        //    //Add new applicationUser
        //    var userModel = new UserWithoutDetailsDTO
        //    {
        //        FirstName = model.FirstName,
        //        LastName = model.LastName,
        //        Email = model.Email,
        //        DOB = model.DOB,
        //        LGA = model.LGA,
        //        PhoneNumber = model.PhoneNumber,
        //        PlaceOfBirth = model.PlaceOfBirth,
        //        State = model.State,
        //        Gender = model.Gender,
        //        HomeAddress = model.HomeAddress
        //    };

        //    var authSupportService = new AuthSupportService(_userManager, _agentRepository, _bankRepository);
        //    var result = await authSupportService.CreateAppUser(userModel, "Agent");
        //    if (!result.Succeeded)
        //    {
        //        foreach (var err in result.Errors)
        //        {
        //            ModelState.AddModelError("", err.Description);
        //        }
        //        return BadRequest(ModelState);
        //    }


        //    //Add field agent
        //    ApplicationUser createdUser = await _userManager.FindByEmailAsync(model.Email);
        //    bool isAgentCreated = false;
        //    if (createdUser == null)
        //        return BadRequest(ResponseMessage.Message("Failed to create identity user"));

        //    try
        //    {
        //        isAgentCreated = await authSupportService.CreateFieldAgent(createdUser.Id, model);
        //    }
        //    catch (Exception e)
        //    {
        //        await _userManager.DeleteAsync(createdUser);
        //        await _userManager.RemoveFromRoleAsync(createdUser, "Agent");
        //        _logger.LogError(e.Message);
        //        return BadRequest(ResponseMessage.Message("Failed to add additional details"));
        //    }


        //    // Add bank details
        //    bool isBankCreated = false;
        //    FieldAgent newCreatedAgent = null;
        //    if (!isAgentCreated)
        //        return BadRequest(("Failed to create user"));

        //    try
        //    {
        //        newCreatedAgent = await _agentRepository.GetAgentById(createdUser.Id);
        //        isBankCreated = await authSupportService.CreateBankDetails(newCreatedAgent.FieldAgentId, model);
        //    }
        //    catch (Exception e)
        //    {
        //        await _userManager.DeleteAsync(createdUser);
        //        await _userManager.RemoveFromRoleAsync(createdUser, "Agent");
        //        await _agentRepository.DeleteAgent(newCreatedAgent);
        //        _logger.LogError(e.Message);
        //        return BadRequest(ResponseMessage.Message("Failed to add bank details"));
        //    }


        //    return Ok(ResponseMessage.Message("Registration was successful!", new { createdUser.Id }));
        //}

    }
}