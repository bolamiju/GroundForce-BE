
using Groundforce.Common.Utilities;
using Groundforce.Services.Models;
using System.Threading.Tasks;
using Groundforce.Services.Data;
using Groundforce.Services.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Groundforce.Services.API.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _ctx;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        public ProfileController(ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, AppDbContext ctx, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _ctx = ctx;
            _cloudinaryConfig = cloudinaryConfig;
        }


        //updates profile picture
        [HttpPatch]
        [Route("{userId}/picture")]
        public async Task<IActionResult> UpdatePicture(string userId, IFormFile picture)
        {
            var userToUpdate = await _userManager.FindByIdAsync(userId);
            if (userToUpdate == null)
            {
                return BadRequest("User does not exist");
            }

            if (picture != null && picture.Length > 0)
            {
                try
                {
                    var photoServices = new PhotoServices(_cloudinaryConfig);
                    userToUpdate.AvatarUrl = photoServices.UploadAvatar(picture);
                    await _userManager.UpdateAsync(userToUpdate);

                    return Ok("Picture successfully uploaded");
                }
                catch (Exception)
                {
                    return BadRequest("Picture not successfully uploaded");
                }
            }
            return BadRequest("Picture not uploaded");
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
