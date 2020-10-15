using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Groundforce.Services.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Groundforce.Services.Models;
using Groundforce.Services.DTOs;
using Microsoft.AspNetCore.Authorization;
using Groundforce.Common.Utilities;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // private fields
        private readonly IConfiguration _config;
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _ctx;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(IConfiguration configuration, ILogger<AccountController> logger, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, AppDbContext ctx, IWebHostEnvironment webHostEnvironment)
        {
            _config = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _ctx = ctx;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // register user
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserToRegisterDTO model)
        {
            var userToAdd = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);

            if (userToAdd != null)
                return BadRequest("Email already exist");

            //create new applicationUser
            var user = new ApplicationUser
            {
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DOB = model.DOB,
                LGA = model.LGA,
                PhoneNumber = model.PhoneNumber,
                PlaceOfBirth = model.PlaceOfBirth,
                State = model.State,
                CreatedAt = DateTime.Now,
                Gender = model.Gender,
                HomeAddress = model.HomeAddress,
            };

            var result = await _userManager.CreateAsync(user, model.PIN);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return BadRequest("Failed to create user!");
            }

            await _userManager.AddToRoleAsync(user, "Agent");

            var createdUser = await _userManager.FindByEmailAsync(model.Email);

            if (createdUser == null) return BadRequest();

            //create new field agent
            var agent = new FieldAgent
            {
                ApplicationUserId = createdUser.Id,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Religion = model.Religion,
                AdditionalPhoneNumber = model.AdditionalPhoneNumber
            };

            try
            {
                await _ctx.FieldAgents.AddAsync(agent);
                _ctx.SaveChanges();
            }
            catch (Exception e)
            {
                _ctx.Remove(createdUser);
                _ctx.SaveChanges();
                _logger.LogError(e.Message);
                return BadRequest("Failed to add additional details");
            }

            var createdFieldAgent = _ctx.FieldAgents.Where(x => x.ApplicationUserId == createdUser.Id).FirstOrDefault();

            if (createdFieldAgent == null) return BadRequest();

            var bank = new BankAccount
            {
                FieldAgentId = createdFieldAgent.FieldAgentId,
                BankName = model.BankName,
                AccountNumber = model.AccountNumber
            };

            try
            {
                await _ctx.BankAccounts.AddAsync(bank);
                //get the phone number of the successfully registered user 
                var registeredUser = _ctx.Request.FirstOrDefault(item => item.PhoneNumber == model.PhoneNumber);
                //set that the user is now verified
                registeredUser.IsVerified = true;
                _ctx.Request.Update(registeredUser);
                await _ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _ctx.Remove(createdUser);
                _ctx.Remove(createdFieldAgent);
                _ctx.SaveChanges();
                _logger.LogError(e.Message);
                return BadRequest("Failed to add bank details");
            }

            return Ok();
        }

        //User Login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
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
                var userRole = await _userManager.GetRolesAsync(user);
                if (result.Succeeded)
                {
                    var getToken = GetTokenHelperClass.GetToken(user, _config, userRole[0]);
                    return Ok(getToken);
                }

                ModelState.AddModelError("", "Invalid creadentials");
                return Unauthorized(ModelState);

            }

            return BadRequest(model);
        }

        //change pin
        [HttpPatch]
        [Route("changePin")]
        public async Task<IActionResult> ChangePin([FromBody] ResetUserPwdDTO userToUpdate)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userToUpdate.UserId);

                if (user == null) return NotFound();

                var updatePwd = await _userManager.ChangePasswordAsync(user, userToUpdate.CurrentPwd, userToUpdate.NewPwd);

                if (updatePwd.Succeeded) return Ok();

                foreach (var error in updatePwd.Errors)
                {
                    ModelState.AddModelError("", $"{error.Code} - {error.Description}");
                }
            }

            return BadRequest(ModelState);
        }

    }
}