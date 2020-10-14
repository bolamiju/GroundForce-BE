using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Services.Data;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Groundforce.Services.API.Controllers
{
    [Authorize(Roles ="Agent")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _ctx;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(UserManager<ApplicationUser> userManager, AppDbContext ctx, ILogger<ProfileController>logger)
        {
            _userManager = userManager;
            _ctx = ctx;
            _logger = logger;
        }
        // Gets the profile of a particular field agent by userID.
        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetProfile(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest();

            var user = await _userManager.FindByIdAsync(userId);


            if (user == null)
            {

                return NotFound("User not found");
            }

            // Returns the field agent by userId
            var agent = await _ctx.FieldAgents.FirstOrDefaultAsync(a => a.ApplicationUserId == userId);

            //  Returns the bank account of that particular field agent using the fieldAgentID
            var bank = await _ctx.BankAccounts.FirstOrDefaultAsync(a => a.FieldAgentId == agent.FieldAgentId);

            var profile = new UserProfileDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DOB = user.DOB,
                Gender = user.Gender,
                Religion = agent.Religion,
                Email = user.Email,
                AdditionalPhoneNumber = agent.AdditionalPhoneNumber,
                ResidentialAddress = user.HomeAddress,
                BankName = bank.BankName,
                AccountNumber = bank.AccountNumber,
            };

            return Ok(profile);
        }

        //update profile controller
        [HttpPut]
        [Route("{Id}")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDTO model, string Id)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(Id);
                if (user == null) return BadRequest("User Does Not Exist");
                //update application user
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.DOB = model.DOB;
                user.Email = model.Email;
                user.Gender = model.Gender;


                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return BadRequest("Failed to update user!");
                }

                int fieldAgentId;
                try
                {
                    //update field agent
                    var agent = await _ctx.FieldAgents.FirstOrDefaultAsync(x => x.ApplicationUserId == Id);
                    agent.AdditionalPhoneNumber = model.AdditionalPhoneNumber;
                    agent.Religion = model.Religion;
                    fieldAgentId = agent.FieldAgentId;
                    _ctx.SaveChanges();
                    
                }
                catch (Exception e)
                {
                    _ctx.SaveChanges();
                    _logger.LogError(e.Message);
                    return BadRequest("Failed to update additional details");
                }

                try
                {
                    //update bank
                    var bank = await _ctx.BankAccounts.FirstOrDefaultAsync(x => x.FieldAgentId == fieldAgentId);
                    bank.BankName = model.BankName;
                    bank.AccountNumber = model.AccountNumber;
                    _ctx.SaveChanges();
                }
                catch(Exception e)
                {
                    _ctx.SaveChanges();
                    _logger.LogError(e.Message);
                    return BadRequest("failed to update bank details");
                }

                return Ok("Success");

            }
            return BadRequest(ModelState);
        }

    }
}
