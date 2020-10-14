using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Groundforce.Common.Utilities;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Groundforce.Services.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PhotoService _photoService;
        private IConfiguration _config;
        private SignInManager<ApplicationUser> _signInManager;
        private AppDbContext _ctx;
        private IWebHostEnvironment _webHostEnvironment;

        public ProfileController(ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, IOptions<CloudinarySettings> cloudinaryConfig, IConfiguration configuration, SignInManager<ApplicationUser> signInManager, AppDbContext ctx, IWebHostEnvironment webHostEnvironment)
        {
            _config = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _ctx = ctx;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;

            _photoService = new PhotoService(cloudinaryConfig);
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

        [HttpPatch("{userId}/picture")]
        public async Task<IActionResult> UploadPicture([FromForm] PhotoForCreation photoFile, string userId)
        {
            if (ModelState.IsValid)
            {
                // get user whose photo is to be updated
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest("User not found");
                }

                var file = photoFile.PhotoFile;
                var uploadResult = new ImageUploadResult();

                if (file.Length > 0)
                {
                    try
                    {
                        uploadResult = _photoService.Upload(file);
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }

                    // update the user photo
                    user.AvatarUrl = uploadResult.Url.ToString();
                    user.PublicId = uploadResult.PublicId;
                    var result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        foreach (var err in result.Errors)
                        {
                            ModelState.AddModelError("", err.Description);
                        }

                        return BadRequest(ModelState);
                    }

                    return Ok("Photo uploaded");
                }

                return BadRequest("No File Found");
            }

            return BadRequest(ModelState);
        }
    }
}