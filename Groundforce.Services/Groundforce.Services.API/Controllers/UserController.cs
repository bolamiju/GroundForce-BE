
using Groundforce.Common.Utilities.Helpers;
using Groundforce.Services.Models;
using System.Threading.Tasks;
using Groundforce.Services.Data;
using Groundforce.Services.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Groundforce.Services.Data.Services;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _ctx;
        private readonly ILogger<UserController> _logger;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IAgentRepository _agentRepository;

        public UserController(ILogger<UserController> logger, UserManager<ApplicationUser> userManager, 
            AppDbContext ctx, IOptions<CloudinarySettings> cloudinaryConfig, IAgentRepository agentRepository )
        {
            _userManager = userManager;
            _ctx = ctx;
            _cloudinaryConfig = cloudinaryConfig;
            _agentRepository = agentRepository;
            _logger = logger;
        }


        //updates user picture
        [HttpPatch]
        [Route("{Id}/picture")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> UpdatePicture(string Id, [FromForm] PhotoToUploadDTO Picture)
        {
            var userToUpdate = await _userManager.FindByIdAsync(Id);
            if (userToUpdate == null)
            {
                return BadRequest("User does not exist");
            }

            var picture = Picture.Photo;

            if (picture != null && picture.Length > 0)
            {
                try
                {
                    var managePhoto = new ManagePhoto(_cloudinaryConfig);
                    var uplResult = managePhoto.UploadAvatar(picture);
                    userToUpdate.AvatarUrl = uplResult.Url.ToString();
                    userToUpdate.PublicId = uplResult.PublicId;
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

        // Gets user by Id.
        [HttpGet]
        [Route("{Id}")]
        [Authorize(Roles = "Agent, Admin")]
        public async Task<IActionResult> Get(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(Id);


            if (user == null)
            {

                return NotFound("User not found");
            }

            // Returns the field agent by userId
            var agent = await _ctx.FieldAgents.FirstOrDefaultAsync(a => a.ApplicationUserId == Id);
            if (agent == null) return NotFound("User's extended details not found");

            //  Returns the bank account of that particular field agent using the fieldAgentID
            var bank = await _ctx.BankAccounts.FirstOrDefaultAsync(a => a.FieldAgentId == agent.FieldAgentId);
            if (bank == null) return NotFound("User's bank details not found");

            var profile = new UserToReturnDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DOB = user.DOB,
                Gender = user.Gender,
                Religion = agent.Religion,
                Email = user.Email,
                AdditionalPhoneNumber = agent.AdditionalPhoneNumber,
                HomeAddress = user.HomeAddress,
                BankName = bank.AccountName,
                AccountNumber = bank.AccountNumber,
                AvatarUrl = user.AvatarUrl,
                PublicId = user.PublicId
            };

            return Ok(profile);
        }

        // edit field agent
        [HttpPut]
        [Route("{Id}")]
        [Authorize(Roles ="Agent")]
        public async Task<IActionResult> EditUser([FromBody] UserToEditDTO model, string Id)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(Id);
                if (user == null) return BadRequest("User Does Not Exist");
                //update application user
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Gender = model.Gender;


                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return BadRequest("Failed to update user!");
                }

                string fieldAgentId;
                try
                {
                    //update field agent
                    var agent = await _agentRepository.GetAgentById(Id);
                    agent.AdditionalPhoneNumber = model.AdditionalPhoneNumber;
                    agent.Religion = model.Religion;
                    fieldAgentId = agent.FieldAgentId;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest("Failed to update additional details");
                }

                return Ok("Success");

            }
            return BadRequest(ModelState);
        }


        //change pin
        [HttpPatch("{Id}/ChangePassword")]
        public async Task<IActionResult> ChangePassword(string Id, [FromBody] ResetUserPwdDTO userToUpdate)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {

                var updatePwd = await _userManager.ChangePasswordAsync(user, userToUpdate.CurrentPwd, userToUpdate.NewPwd);

                if (updatePwd.Succeeded) return Ok("Password Changed!");

                foreach (var error in updatePwd.Errors)
                {
                    ModelState.AddModelError("", $"{error.Code} - {error.Description}");
                }
            }

            return BadRequest(ModelState);
        }

    }
}
