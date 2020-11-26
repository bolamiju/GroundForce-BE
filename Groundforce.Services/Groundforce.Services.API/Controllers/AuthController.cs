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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;
using System.Data.Common;

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
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly IMailService _mailService;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAgentRepository _agentRepository;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger,
                                 SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager, AppDbContext ctx,
                                 IRequestRepository requestRepository, IAgentRepository agentRepository,
                                 IEmailVerificationRepository emailVerificationRepository, IMailService mailService)
        {
            _config = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _ctx = ctx;
            _requestRepository = requestRepository;
            _emailVerificationRepository = emailVerificationRepository;
            _mailService = mailService;
            _userManager = userManager;
            _agentRepository = agentRepository;
        }

        //verify OTP
        [HttpPost("verify-phone")]
        public async Task<IActionResult> VerifyPhone([FromBody] PhoneNumberToVerifyDTO model)
        {
            if (String.IsNullOrWhiteSpace(model.PhoneNumber)) return BadRequest(ResponseMessage.Message("Bad request", errors: new { message ="Invalid request credential" }));

            Request number = null;
            try
            {
                // fetch records related to phone number if it already exists in the table
                number = await _requestRepository.GetRequestByPhone(model.PhoneNumber);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Could not access record related to phone number" }));
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
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = e.Message }));
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
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Error with database processing" }));
            }

            try
            {
                CreateTwilioService.Init(_config);
                await CreateTwilioService.SendOTP(model.PhoneNumber);
                return Ok(ResponseMessage.Message("Success", data: new { message = "OTP sent!" }));
            }
            catch (TwilioException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to send OTP" }));
            }
        }


        // confirm OTP
        [HttpPost("confirm-otp")]
        public async Task<IActionResult> ConfirmOTP([FromBody] OTPToConfirmDTO model)
        {
            if (String.IsNullOrWhiteSpace(model.PhoneNumber)) return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Invalid request credentials" }));

            Request number = null;
            try
            {
                // fetch records related to phone number if it already exists in the table
                number = await _requestRepository.GetRequestByPhone(model.PhoneNumber);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Could not access record related to phone number" }));
            }

            // if phone number has gone through verification
            if (number == null) return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Phone number has not gone through verification yet" }));


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
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = e.Message }));
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
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to confirm OTP try requesting for a new OTP" }));
            }

            try
            {
                if (status == PhoneNumberStatus.pending.ToString().ToLower())
                {
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "OTP does not match" }));
                }

                number.Status = "confirmed";
                if (!await _requestRepository.UpdateRequest(number))
                    throw new Exception("Could not update request");
                return Ok(ResponseMessage.Message("Success", data: new { message = "OTP confirmed!" }));

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Error with database processing" }));
            }

        }


        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromForm] EmailToVerifyDTO model)
        {
            string emailCode;
            try
            {
                emailCode = Guid.NewGuid().ToString();
                emailCode = Regex.Replace(emailCode, @"\D", "");

                if (emailCode.Length > 4) emailCode = emailCode.Remove(4);
                else if (emailCode.Length < 4)
                {
                    while (emailCode.Length != 4)
                    {
                        Random rd = new Random();
                        emailCode += rd.Next(0, 9);
                    }
                }

                int.TryParse(emailCode, out int code);

                var response = await _emailVerificationRepository.GetEmailVerificationByEmail(model.EmailAddress);
                if (response != null)
                {
                    response.VerificationCode = emailCode;
                    await _emailVerificationRepository.UpdateEmailVerification(response);
                }
                else if(response != null && response.IsVerified)
                {
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "User is already verified" }));
                }
                else
                {
                    // generate email verification id
                    string emailVerificatioinId;
                    EmailVerification result;
                    do
                    {
                        emailVerificatioinId = Guid.NewGuid().ToString();
                        result = await _emailVerificationRepository.GetEmailVerificationById(emailVerificatioinId);
                    } while (result != null);

                    var email = new EmailVerification
                    {
                        Id = emailVerificatioinId,
                        EmailAddress = model.EmailAddress,
                        VerificationCode = emailCode
                    };

                    await _emailVerificationRepository.AddEmailVerification(email);
                }

            }
            catch (DbException de)
            {
                _logger.LogError(de.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to add email verification" }));
            }

            try
            {
                string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                var request = new MailRequest
                {
                    GroundForceUrl = baseUrl,
                    ToEmail = model.EmailAddress,
                    Content = "Verify Email Template.",
                    IsHidden = true,
                    MainHeader = "Your email verification code",
                    SubHeader = emailCode
                };

                await _mailService.SendMailAsync(request);

                return Ok(ResponseMessage.Message("Ok", data: new { message = "Email successfully sent" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to send email address" }));
            }
        }

        //confirm email
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromForm] EmailToConfirmDTO email)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseMessage.Message("Wrong input", errors: new { message = "Please enter a valid email address" }));
            EmailVerification result;

            try
            {
                result = await _emailVerificationRepository.GetEmailVerificationByEmail(email.EmailAddress);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not find the email address" }));
            }

            if (result == null) return BadRequest(ResponseMessage.Message("Email does not exist", errors: new { message = email.EmailAddress }));

            if (result.VerificationCode == email.VerificationCode)
            {
                try
                {
                    result.IsVerified = true;
                    await _emailVerificationRepository.UpdateEmailVerification(result);
                    return Ok(ResponseMessage.Message("Success.", data: new { message = "Email has been successfully confirmed" }));
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "Could not confirm the email." }));
                }
            }
            return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "Code provided does not match" }));
        }

        // forgot password route
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.EmailAddress);
            if(user == null) return NotFound(ResponseMessage.Message("Not Found", errors: new { message = "Email does not exist" })); 

            try
            {
                // Use user to generate token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Use token to genetate password reset link
                var emailUrl = Url.Action("ResetPassword", "Auth", new { email = model.EmailAddress, token }, Request.Scheme);
                string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                var forgotPassword = new MailRequest
                {
                    ToEmail = model.EmailAddress,
                    Link = emailUrl,
                    GroundForceUrl = baseUrl,
                    Content = "Reset Password Email Template.",
                    IsHidden = false,
                    ButtonName = "Reset Password",
                    MainHeader = "You have requested to reset your password",
                    SubHeader = " A unique link to reset your password has been generated for you. Click here"
                };
                await _mailService.SendMailAsync(forgotPassword);
                return Ok(ResponseMessage.Message("Ok", data: new { message = "Forgot password reset link was successfully sent" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Forgot password link failed to send" }));
            }
        }


        //reset password
        [HttpPatch("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                //check if the user exists in the table by email
                var user = _userManager.Users.SingleOrDefault(x => x.Email == model.Email);

                if (user == null)
                    return NotFound(ResponseMessage.Message("Bad request", errors: new { message = $"User with email: {model.Email}, is not found" }));

                if (!user.IsActive)
                    return Unauthorized(ResponseMessage.Message("Unauthorized", errors: new { message = "In-active account" }));

                try
                {
                    // reset user password
                    var setNewPassword = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                    if (setNewPassword.Succeeded)
                        return Ok(ResponseMessage.Message("Success", data: new { message = $"Password for {model.Email} is successfully updated" }));
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = $"Could not update password for {model.Email}" }));
                }
            }
            return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "Invalid input value" }));
        }


        // register user
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserToRegisterDTO model)
        {

            try
            {
                // ensure that number has gone through verification and confirmation
                var phoneNumberIsInRequestTable = await _requestRepository.GetRequestByPhone(model.PhoneNumber);
                if (phoneNumberIsInRequestTable == null)
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Phone number has not gone through verification yet" }));

                if (phoneNumberIsInRequestTable.Status == "pending")
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Phone number has not been confirmed yet" }));

                //var emailIsInEmailVerificationTable = await _emailVerificationRepository.GetEmailVerificationByEmail(model.Email);
                //if (emailIsInEmailVerificationTable == null)
                //    return BadRequest(ResponseMessage.Message("Bad request", errors: "Email address has not gone through verification yet"));
                //if (emailIsInEmailVerificationTable != null && !emailIsInEmailVerificationTable.IsVerified)
                //    return BadRequest(ResponseMessage.Message("Bad request", errors: "Email address has not verified yet"));

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Data processing error" }));
            }

            ApplicationUser createdUser = null;
            try
            {
                // check if email aready exists
                var emailToAdd = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);
                if (emailToAdd != null)
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Email already exist" }));

                // check if number aready exists
                var numberToAdd = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == model.PhoneNumber);
                if (numberToAdd != null)
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Phone number already exist" }));

                // convert list to lowercase
                var convertedList = Util.ListToLowerCase(model.Roles);

                if (convertedList.Contains("admin") || convertedList.Contains("client"))
                {
                    if (!User.Identity.IsAuthenticated)
                        return Unauthorized(ResponseMessage.Message("Unauthorized", errors: new { message = "User must be signed-in, to register other users" }));

                    if (!User.IsInRole("Admin"))
                        return Unauthorized(ResponseMessage.Message("Unauthorized", errors: new { message = "User must be an Admin to perform this task" }));

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
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = ModelState }));
                }

                // create agent
                createdUser = await _userManager.FindByEmailAsync(model.Email);
                if (createdUser == null)
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to create identity user" }));

                bool successResult = false;
                if (convertedList.Contains("agent"))
                {
                    successResult = await auth.CreateFieldAgent(model, createdUser.Id);
                    if (!successResult)
                    {
                        await _userManager.DeleteAsync(createdUser);
                        return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to create user" }));
                    }
                }

                //try
                //{
                //    if (successResult)
                //    {
                //        string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                //        var request = new MailRequest
                //        {
                //            GroundForceUrl = baseUrl,
                //            ToEmail = model.Email,
                //            Content = "Welcome Email Template.",
                //            IsHidden = true,
                //            MainHeader = "Welcome to Ground Force",
                //            SubHeader = $"Hello {model.FirstName}, you have successfully registered on our platform. Welcome Onboard!!!"
                //        };

                //        await _mailService.SendMailAsync(request);
                //    }
                //}
                //catch (Exception e)
                //{
                //    _logger.LogError(e.Message);
                //    return BadRequest(ResponseMessage.Message("Bad request", errors: "Could not send welcome mail"));
                //}

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Data processing error" }));
            }

            return Ok(ResponseMessage.Message("Success. Welcome mail was sent", data: new { createdUser.Id }));

        }


        //User Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserToLoginDTO model)
        {

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = ModelState }));

                //get user by email
                var user = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);

                //Check if user exist
                if (user == null)
                {
                    return Unauthorized(ResponseMessage.Message("Unauthorized", errors: new { message = "Invalid credentials" }));
                }

                if (!user.IsActive)
                    return Unauthorized(ResponseMessage.Message("Unauthorized", errors: new { message = "In-active account" }));

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                var userRoles = await _userManager.GetRolesAsync(user);
                if (result.Succeeded)
                {
                    var getToken = JwtTokenConfig.GetToken(user, _config, userRoles);
                    return Ok(ResponseMessage.Message("Success", data: new { token = getToken }));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Data processing error" }));
            }

            return Unauthorized(ResponseMessage.Message("Unauthorized", errors: new { message = "Invalid credentials" }));

        }

    }
}