using System;
using System.Threading.Tasks;
using Groundforce.Services.DTOs;
using Groundforce.Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // private fields
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

        //confirm OTP
        // api/v1/confirmation
        [HttpPost("confirmation")]
        public async Task<IActionResult> Confirmation([FromBody] ConfirmationDTO model)
        {
            string response = await CreateTwilioService.ConfirmOTP(model.PhoneNumber, model.VerifyCode);

            if (response.Equals("approved"))
            {
                return Ok();
            }
            else
            {
                return BadRequest(response);
            }
        }

//<<<<<<< HEAD
//        // register user
//        [HttpPost("signup")]
//        public async Task<IActionResult> SignUp(UserToRegisterDTO model)
//        {
//            var userToAdd = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);

//            if (userToAdd != null)
//                return BadRequest("Email already exist");

//            //create new applicationUser
//            var user = new ApplicationUser
//            {
//                UserName = model.Email,
//                FirstName = model.FirstName,
//                LastName = model.LastName,
//                Email = model.Email,
//                DOB = model.DOB,
//                LGA = model.LGA,
//                PlaceOfBirth = model.PlaceOfBirth,
//                State = model.State,
//                CreatedAt = DateTime.Now,
//                Gender = model.Gender,
//                HomeAddress = model.HomeAddress,
//            };

//            var result = await _userManager.CreateAsync(user, model.PIN);

//            if (!result.Succeeded)
//            {
//                foreach (var err in result.Errors)
//                {
//                    ModelState.AddModelError("", err.Description);
//                }
//                return BadRequest(StatusCodes.Status400BadRequest);
//            }

//            await _userManager.AddToRoleAsync(user, "Agent");

//            var createdUser = await _userManager.FindByEmailAsync(model.Email);

//            if (createdUser == null) return BadRequest();

//            //create new field agent
//            var agent = new FieldAgent
//            {
//                ApplicationUserId = createdUser.Id,
//                Latitude = model.Latitude,
//                Longitude = model.Longitude,
//                Religion = model.Religion,
//                AdditionalPhoneNumber = model.AdditionalPhoneNumber
//            };

//            try
//            {
//                await _ctx.FieldAgents.AddAsync(agent);
//            }
//            catch (Exception)
//            {
//                _ctx.Remove(createdUser);
//                _ctx.SaveChanges();
//                return BadRequest(StatusCodes.Status400BadRequest);
//            }

//            var createdFieldAgent = _ctx.FieldAgents.Where(x => x.ApplicationUserId == createdUser.Id).FirstOrDefault();

//            if (createdFieldAgent == null) return BadRequest();

//            var bank = new BankAccount
//            {
//                FieldAgentId = createdFieldAgent.FieldAgentId,
//                BankName = model.BankName,
//                AccountNumber = model.AccountNumber
//            };

//            try
//            {
//                await _ctx.BankAccounts.AddAsync(bank);
//            }
//            catch (Exception)
//            {
//                _ctx.Remove(createdUser);
//                _ctx.Remove(createdFieldAgent);
//                _ctx.SaveChanges();
//                return BadRequest(StatusCodes.Status400BadRequest);
//            }

//            await _ctx.SaveChangesAsync();

//            return StatusCode(201);
//        }

//        //User Login
//        [AllowAnonymous]
//        [HttpPost("Login")]
//        public async Task<IActionResult> Login(LoginDTO model)
//        {
//            if (ModelState.IsValid)
//            {

//                //get user by email
//                var user = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);

//                //Check if user exist
//                if (user == null)
//                {
//                    return BadRequest("Account does not exist");
//                }

//                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Pin, false, false);

//                if (result.Succeeded)
//                {
//                    var tokenGetter = new GetTokenHelperClass();
//                    var getToken = tokenGetter.GetToken(user, _config);

//                    return Ok(getToken);
//                }
//                else
//                {
//                    return Unauthorized("Invalid creadentials");
//                }
//            }
//            else
//            {
//                return BadRequest("Enter valid credentials");
//            }

//        }

//=======
//>>>>>>> 38ec4912b45cf7a03ad93d295ddab31111efc16e
    }
}
