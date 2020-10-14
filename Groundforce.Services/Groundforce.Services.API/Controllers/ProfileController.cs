using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Services.Models;
using Groundforce.Services.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Groundforce.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Groundforce.Services.API.Controllers
{
    //[Authorize (Roles="Agent")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _ctx;


        public ProfileController(ILogger<ProfileController>logger, UserManager<ApplicationUser> userManager, AppDbContext ctx)
        {
            _logger = logger;
            _userManager = userManager;
            _ctx = ctx;
        }


        //update profile controller
        [HttpPut]
        [Route("updateprofile")]
        public async Task<IActionResult> UpdateProfile([FromBody] updateProfileDTO model, string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null) return BadRequest("User Does Not Exist");


            if (ModelState.IsValid)
            {

                //update application user
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.DOB = model.DOB;
                user.Email = model.Email;
                user.Gender = model.Gender;
                
             
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded) return BadRequest("An Error Occured");

                try
                {
                    //update field agent
                    var agent = await _ctx.FieldAgents.FirstOrDefaultAsync(x => x.ApplicationUserId == Id);
                    agent.AdditionalPhoneNumber = model.AdditionalPhoneNumber;
                    agent.Religion = model.Religion;
                    _ctx.SaveChanges();
                    //update bank
                    var bank = await _ctx.BankAccounts.FirstOrDefaultAsync(x => x.FieldAgentId == agent.FieldAgentId);
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


    }
}
