using System;
using System.Threading.Tasks;
using Groundforce.Common.Utilities;
using Groundforce.Services.Core;
using Groundforce.Services.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio.Exceptions;
using Microsoft.AspNetCore.Identity;
using Groundforce.Services.Models;
using Groundforce.Services.DTOs;

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


        public AuthController(IConfiguration configuration, ILogger<AuthController> logger,
                                 SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager, AppDbContext ctx)
        {
            _config = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _ctx = ctx;
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
                var phoneNumberResource = new PhoneNumberResource(_ctx);
                //call the phone number check method
                phoneNumberStatus = await phoneNumberResource.CheckPhoneNumber(model.PhoneNumber);
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
                return Ok("OTP confirmed!");
            }
            catch (TwilioException e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Failed to confirm OTP");
            }
        }


        //// register user
        //[HttpPost("signup")]
        //public async Task<IActionResult> SignUp(UserToRegisterDTO model)
        //{
        //    var userToAdd = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);
        //    if (userToAdd != null)
        //        return BadRequest("Email already exist");

        //    var phoneNumberIsInRequestTable = await _ctx.Request.AnyAsync(x => x.PhoneNumber == model.PhoneNumber);
        //    if (!phoneNumberIsInRequestTable)
        //        return BadRequest("Phone number has not gone through verification process");

        //    //create new applicationUser
        //    string defaultPix = "~/images/avarta.jpg";
        //    var user = new ApplicationUser
        //    {
        //        UserName = model.Email,
        //        FirstName = model.FirstName,
        //        LastName = model.LastName,
        //        Email = model.Email,
        //        DOB = model.DOB,
        //        LGA = model.LGA,
        //        PhoneNumber = model.PhoneNumber,
        //        PlaceOfBirth = model.PlaceOfBirth,
        //        State = model.State,
        //        CreatedAt = DateTime.Now,
        //        Gender = model.Gender,
        //        HomeAddress = model.HomeAddress,
        //        AvatarUrl = defaultPix
        //    };

        //    var result = await _userManager.CreateAsync(user, model.PIN);

        //    if (!result.Succeeded)
        //    {
        //        foreach (var err in result.Errors)
        //        {
        //            ModelState.AddModelError("", err.Description);
        //        }
        //        return BadRequest("Failed to create user!");
        //    }

        //    await _userManager.AddToRoleAsync(user, "Agent");

        //    var createdUser = await _userManager.FindByEmailAsync(model.Email);

        //    if (createdUser == null) return BadRequest();

        //    //create new field agent
        //    var agent = new FieldAgent
        //    {
        //        ApplicationUserId = createdUser.Id,
        //        Latitude = model.Latitude,    
        //        Longitude = model.Longitude,
        //        Religion = model.Religion,
        //        AdditionalPhoneNumber = model.AdditionalPhoneNumber
        //    };

        //    try
        //    {
        //        await _ctx.FieldAgents.AddAsync(agent);
        //        _ctx.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        _ctx.Remove(createdUser);
        //        _ctx.SaveChanges();
        //        _logger.LogError(e.Message);
        //        return BadRequest("Failed to add additional details");
        //    }

        //    var createdFieldAgent = _ctx.FieldAgents.Where(x => x.ApplicationUserId == createdUser.Id).FirstOrDefault();

        //    if (createdFieldAgent == null) return BadRequest();

        //    var bank = new BankAccount
        //    {
        //        FieldAgentId = createdFieldAgent.FieldAgentId,
        //        BankName = model.BankName,
        //        AccountNumber = model.AccountNumber
        //    };

        //    try
        //    {
        //        await _ctx.BankAccounts.AddAsync(bank);
        //        //get the phone number of the successfully registered user 
        //        var registeredUser = _ctx.Request.FirstOrDefault(item => item.PhoneNumber == model.PhoneNumber);
        //        //set that the user is now verified
        //        registeredUser.IsVerified = true;
        //        _ctx.Request.Update(registeredUser);
        //        await _ctx.SaveChangesAsync();
        //    }
        //    catch (Exception e)
        //    {
        //        _ctx.Remove(createdUser);
        //        _ctx.Remove(createdFieldAgent);
        //        _ctx.SaveChanges();
        //        _logger.LogError(e.Message);
        //        return BadRequest("Failed to add bank details");
        //    }

        //    return Ok();
        //}

        ////User Login
        //[HttpPost("login")]
        //public async Task<IActionResult> Login(LoginDTO model)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        //get user by email
        //        var user = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);

        //        //Check if user exist
        //        if (user == null)
        //        {
        //            return BadRequest("Account does not exist");
        //        }

        //        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Pin, false, false);
        //        var userRoles = await _userManager.GetRolesAsync(user);
        //        if (result.Succeeded)
        //        {
        //            var getToken = GetTokenHelperClass.GetToken(user, _config, userRoles);
        //            return Ok(getToken);
        //        }

        //        ModelState.AddModelError("", "Invalid creadentials");
        //        return Unauthorized(ModelState);

        //    }

        //    return BadRequest(model);
        //}


        //// forgot password route
        //[HttpPatch("ForgotPassword")]
        //public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO details)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = _userManager.Users.SingleOrDefault(e => e.PhoneNumber == details.PhoneNumber);
        //        if (user == null) return NotFound();
        //        //generate token needed to reset password
        //        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        //        var setNewPassword = await _userManager.ResetPasswordAsync(user, token, details.Pin);
        //        if (setNewPassword.Succeeded) return Ok("Password successfully updated");

        //        // if passwordset is unsuccessful add errors to model error
        //        foreach (var error in setNewPassword.Errors)
        //        {
        //            ModelState.AddModelError("Error", error.Description);
        //        }
        //    }
        //    return BadRequest();
        //}

    }
}