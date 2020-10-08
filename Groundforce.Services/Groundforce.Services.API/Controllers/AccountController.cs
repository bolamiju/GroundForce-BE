using System.Linq;
using System.Threading.Tasks;
using Groundforce.Common.Utilities.Util;
using Groundforce.Services.API.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly AuthRepo _AuthRepo;

        public AccountController(ILogger<AccountController>logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config
            )
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _AuthRepo = new AuthRepo(_config);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUsers model)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);

                if (user == null)
                {
                    return BadRequest();
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Pin, false, false);

                if (result.Succeeded)
                {
                    var token = _AuthRepo.GetToken(user.Id, user.LastName);

                    return Ok(token);
                }
                else
                {
                    return Unauthorized("Invalid credentials");
                }
            }
            return BadRequest();
        }

    }
}
