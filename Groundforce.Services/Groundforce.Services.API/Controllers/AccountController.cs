using Groundforce.Common.Utilities;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
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
        public AccountController(ILogger<AccountController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
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
    }
}