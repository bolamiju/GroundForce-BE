using System;
using System.Threading.Tasks;
using Groundforce.Services.Data;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _ctx;

        public ProfileController(ILogger<ProfileController> logger, UserManager<ApplicationUser> userManager, AppDbContext ctx)
        {
            _logger = logger;
            _userManager = userManager;
            _ctx = ctx;
        }

        [HttpPut]
        [Route("updateprofile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO model, string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null) return BadRequest("User Does Not Exist");

            if (ModelState.IsValid)
            {
                //update application user details
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.DOB = model.DOB;
                user.Email = model.Email;
                user.Gender = model.Gender;


                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) return BadRequest("An Error Occured");
                try
                {
                    //update field agent details
                    var agent = await _ctx.FieldAgents.FirstOrDefaultAsync(agent => agent.ApplicationUserId == Id);
                    agent.AdditionalPhoneNumber = model.AdditionalPhoneNumber;
                    agent.Religion = model.Religion;
                    _ctx.SaveChanges();
                    //update bank details
                    var bank = await _ctx.BankAccounts.FirstOrDefaultAsync(bank => bank.FieldAgentId == agent.FieldAgentId);
                    bank.BankName = model.BankName;
                    bank.AccountNumber = model.AccountNumber;
                    _ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    return BadRequest("An Error Occured");
                }
                return Ok("Success");
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        [Route("{userid}")]

        public async Task<IActionResult> FetchUserDetails(string userid)
        {

            // get user with userId
            var user = await _userManager.FindByIdAsync(userid);

            // check if user is null
            if (user == null)
            {
                return NotFound("Account does not exist");
            }

            // get agent  
            var agent = await _ctx.FieldAgents.FirstOrDefaultAsync(x => x.ApplicationUserId == userid);
            // agent accounts
            var bankDetail = await _ctx.BankAccounts.FirstOrDefaultAsync(x => x.FieldAgentId == agent.FieldAgentId);

            // user profile
            var userProfile = new UserprofileDTO();
            userProfile.FirstName = user.FirstName;
            userProfile.LastName = user.LastName;
            userProfile.Address = user.HomeAddress;
            userProfile.DOB = user.DOB;
            userProfile.Email = user.Email;
            userProfile.Gender = user.Gender;
            userProfile.Religion = agent.Religion;
            userProfile.AdditionalPhoneNumber = agent.AdditionalPhoneNumber;
            userProfile.BankName = bankDetail.BankName;
            userProfile.AccountNumber = bankDetail.AccountNumber;

            // return profile 
            return Ok(userProfile);

        }

    }
}
