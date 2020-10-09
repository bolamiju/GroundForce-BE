using Groundforce.Common.Utilities;
using Groundforce.Services.API.DTOs;
using Groundforce.Services.Data;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    //Account controller
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public AccountController(ILogger<AccountController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration config, AppDbContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _context = context;
        }

        //Log in controller  takes  Email and PassWord
        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] userToLoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user == null) return BadRequest("Account not valid");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password.ToString(), false, false);

                if (result.Succeeded)
                {
                    var tokenGet = new TokenGetter();
                    var tokenValue = tokenGet.GetToken(user, _config);
                    return Ok(tokenValue);
                }
                else
                {
                    return Unauthorized("Invalid Email or Pin");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPatch]
        [Route("resetpin")]
        public async Task<IActionResult> resetpin([FromBody] VerifiedUserDTO model)
        {
            if (ModelState.IsValid)
            {
                // get user using user phone number
                var user = await _userManager.FindByIdAsync(model.UserId);
                //if user not found, return notfound
                if (user == null) return NotFound();
                // change user password
                var updatePassword = await _userManager.ChangePasswordAsync(user, model.currentPin, model.newPin);
                if (updatePassword.Succeeded)
                {
                    // return ok status if password updates successfully
                    return Ok();
                }
            }

            // return  Bad Request otherwise
            ModelState.AddModelError("", "Failed to update password");
            return BadRequest();
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp(RegistrationDTO model)
        {
            //check if email already exists
            if (_context.Users.FirstOrDefault(x => x.Email == model.Email) != null)
            {
                return BadRequest("Email Already exist");
            }
            //checks if model state is valid
            if (ModelState.IsValid)
            {
                //creates a new instance of an ApplicationUser
                var user = new ApplicationUser
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Email = model.Email,
                    DOB = model.DOB,
                    Gender = model.Gender,
                    HomeAddress = model.HomeAddress,
                    UserName = model.Email,
                    LGA = model.LGA,
                    PlaceOfBirth = model.PlaceOfBirth,
                    State = model.State
                };
                //creates a user
                var result = await _userManager.CreateAsync(user, model.PIN);
                //checks if user creation was successful
                if (result.Succeeded)
                {
                    //assigns role to the user
                    await _userManager.AddToRoleAsync(user, "Agent");

                    //creats a new instance of a field agent
                    var fieldAgent = new FieldAgent
                    {
                        Longitude = model.Longitude,
                        Latitude = model.Latitude,
                        AdditionalPhoneNumber = model.AdditionalPhoneNumber,
                        Religion = model.Religion,
                        ApplicationUser = _context.Users.FirstOrDefault(x => x.Email == model.Email),
                        ApplicationUserId = _context.Users.FirstOrDefault(x => x.Email == model.Email).Id
                    };

                    try
                    {
                        //adds the agent to the database
                        var agentResult = await _context.FieldAgents.AddAsync(fieldAgent);
                        var agentId = agentResult.Entity.FieldAgentId;
                        _context.SaveChanges();

                    }
                    catch (Exception)
                    {
                        //catches any exception and removes the created user
                        var removeUserResult = await _userManager.DeleteAsync(user);
                        return BadRequest(StatusCodes.Status400BadRequest);
                    }
                    try
                    {
                        //creates a new agent bank account number
                        var agentBankAcc = new BankAccount
                        {
                            BankName = model.BankName,
                            AccountNumber = model.AccountNumber,
                            FieldAgentId = fieldAgent.FieldAgentId,
                            FieldAgent = fieldAgent
                        };
                        //adds the account number to the database
                        _context.BankAccounts.Add(agentBankAcc);
                        _context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        //removes the added user and the added field agent if error arises from saving bank account to the database
                        await _userManager.DeleteAsync(user);
                        _context.FieldAgents.Remove(fieldAgent);
                        _context.SaveChanges();
                        return BadRequest(StatusCodes.Status400BadRequest);

                    }
                    //returns success if user is successfully created
                    return Ok(StatusCodes.Status201Created);
                }
            }
            //returns bad request if model state is not valid
            return BadRequest(StatusCodes.Status400BadRequest);
        }
    }
}