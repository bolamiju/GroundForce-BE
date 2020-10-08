using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Services.API.DTOs;
using Groundforce.Services.Data;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Groundforce.Services.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController(UserManager<ApplicationUser> userManager, AppDbContext dbContext, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] UserToRegister model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                {
                    return BadRequest(ModelState);
                }

                var userToAdd = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);

                if (userToAdd != null)
                {
                    return BadRequest("Email already exist");
                }

                var user = new ApplicationUser()
                {
                    UserName = model.Email,
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Email = model.Email,
                    DOB = model.DateOfBirth.ToLongDateString(),
                    HomeAddress = model.Address,
                    Gender = model.Gender,
                    PhoneNumber = model.PhoneNumber,
                    PlaceOfBirth = model.PlaceOfBirth,
                    LGA = model.LGA,
                    State = model.State
                };

                var result = await _userManager.CreateAsync(user, model.PIN);

                if (!result.Succeeded)
                {
                    //var msg = "";
                    foreach (var err in result.Errors)
                    {
                        //msg += err.Description + " ";
                        ModelState.AddModelError("", err.Description);
                    }

                    return BadRequest(result.Errors);
                }

                //assign role
                await _userManager.AddToRoleAsync(user, "Agent");

                var savedUser = await _userManager.FindByEmailAsync(model.Email);

                // Creating new field agent
                try
                {
                    var newAgent = new FieldAgent()
                    {
                        ApplicationUserId = savedUser.Id,
                        ApplicationUser = savedUser,
                        Longitude = model.Longitude,
                        Latitude = model.Latitude,
                        Religion = model.Religion,
                        AdditionalPhoneNumber = model.AdditionalPhoneNumber
                    };

                    await _dbContext.FieldAgents.AddAsync(newAgent);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    await _userManager.DeleteAsync(savedUser);

                    return BadRequest(e.Message);
                }

                var savedAgent = _dbContext.FieldAgents.FirstOrDefault(agent => agent.ApplicationUserId == savedUser.Id);

                // Creating new bank account
                try
                {
                    if (savedAgent != null)
                    {
                        var newBankAccount = new BankAccount()
                        {
                            FieldAgentId = savedAgent.FieldAgentId,
                            AccountNumber = model.AccountNumber,
                            BankName = model.BankName,
                            FieldAgent = savedAgent
                        };

                        await _dbContext.BankAccounts.AddRangeAsync(newBankAccount);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {
                    await _userManager.DeleteAsync(savedUser);
                    if (savedAgent != null) _dbContext.FieldAgents.Remove(savedAgent);
                    await _dbContext.SaveChangesAsync();

                    return BadRequest(e.Message);
                }

                return Ok(result);
            }
            return BadRequest(ModelState);
        }
    }
}