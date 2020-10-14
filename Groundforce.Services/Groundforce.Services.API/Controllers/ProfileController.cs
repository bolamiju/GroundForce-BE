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
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _ctx;

        public ProfileController(UserManager<ApplicationUser> userManager, AppDbContext ctx)
        {
        
            _userManager = userManager;
            _ctx = ctx;
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
                PhoneNumber = user.PhoneNumber,
                SecondPhoneNumber = agent.AdditionalPhoneNumber,
                Address = user.HomeAddress,
                BankName = bank.BankName,
                AccountNumber = bank.AccountNumber,
                LGA = user.LGA
            };

            return Ok(profile);
        }

    }
}
