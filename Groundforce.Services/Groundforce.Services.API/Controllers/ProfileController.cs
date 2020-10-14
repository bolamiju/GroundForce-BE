using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Services.Data;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private IConfiguration _config;
        private ILogger<AccountController> _logger;
        private SignInManager<ApplicationUser> _signInManager;
        private AppDbContext _ctx;
        private UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment _webHostEnvironment;

        public ProfileController(IConfiguration configuration, ILogger<AccountController> logger, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, AppDbContext ctx, IWebHostEnvironment webHostEnvironment)
        {


            _config = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _ctx = ctx;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;

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
            var userProfile = new UserprofileDTOs();
            userProfile.FirstName = user.FirstName;
            userProfile.LastName = user.LastName;
            userProfile.Address = user.HomeAddress;
            userProfile.DateOfBirth = user.DOB;
            userProfile.Email = user.Email;
            userProfile.Gender = user.Gender;
            userProfile.Religion = agent.Religion;
            userProfile.PhoneNumber2 = agent.AdditionalPhoneNumber;
            userProfile.BankName = bankDetail.BankName;
            userProfile.AccountNumber = bankDetail.AccountNumber;

            // return profile 
            return Ok(userProfile);

        }

    }
}
