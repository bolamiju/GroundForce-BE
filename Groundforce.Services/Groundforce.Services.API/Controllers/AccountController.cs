using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Groundforce.Services.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Groundforce.Services.Models;
using Groundforce.Services.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // private fields
        private IConfiguration _config;
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
                return BadRequest(StatusCodes.Status400BadRequest);
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
            }
            catch (Exception)
            {
                _ctx.Remove(createdUser);
                _ctx.SaveChanges();
                return BadRequest(StatusCodes.Status400BadRequest);
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
            }
            catch (Exception)
            {
                _ctx.Remove(createdUser);
                _ctx.Remove(createdFieldAgent);
                _ctx.SaveChanges();
                return BadRequest(StatusCodes.Status400BadRequest);
            }

            await _ctx.SaveChangesAsync();

            return StatusCode(201);
        }



        [HttpPatch]
        [Route("resetPin")]
        public async Task<IActionResult> resetPin([FromBody] ResetUserPwdDTO userToUpdate)
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

            return BadRequest();
        }
    }
}
