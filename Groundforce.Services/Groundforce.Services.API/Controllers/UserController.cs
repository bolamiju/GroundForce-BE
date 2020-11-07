
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
using System.Collections.Generic;
using System.Linq;
using Groundforce.Services.Core;

namespace Groundforce.Services.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _ctx;
        private readonly ILogger<UserController> _logger;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IAgentRepository _agentRepository;
        private readonly IBankRepository _bankRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IAdminRepository _adminRepository;

        public UserController(ILogger<UserController> logger, UserManager<ApplicationUser> userManager, 
            AppDbContext ctx, IOptions<CloudinarySettings> cloudinaryConfig, IAgentRepository agentRepository,
                                 IAdminRepository adminRepository, IBankRepository bankRepository,
                                 IRequestRepository requestRepository)
        {
            _userManager = userManager;
            _ctx = ctx;
            _cloudinaryConfig = cloudinaryConfig;
            _agentRepository = agentRepository;
            _logger = logger;
            _adminRepository = adminRepository;
            _bankRepository = bankRepository;
            _requestRepository = requestRepository;
        }
               
        //updates user picture
        [HttpPatch]
        [Route("{Id}/picture")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> UpdatePicture(string Id, [FromForm] PhotoToUploadDTO Picture)
        {
            ApplicationUser user = null;
            try
            {
                var authSupportService = new AuthSupportService(_userManager, _agentRepository, _bankRepository);
                user = await authSupportService.verifyUser(Id);
            }
            catch (Exception e)
            {
                return BadRequest(ResponseMessage.Message(e.Message));
            }

            var picture = Picture.Photo;

            if (picture != null && picture.Length > 0)
            {
                try
                {
                    var managePhoto = new ManagePhoto(_cloudinaryConfig);
                    var uplResult = managePhoto.UploadAvatar(picture);
                    user.AvatarUrl = uplResult.Url.ToString();
                    user.PublicId = uplResult.PublicId;
                    await _userManager.UpdateAsync(user);

                    return Ok(ResponseMessage.Message("Picture successfully uploaded"));
                }
                catch (Exception)
                {
                    return BadRequest(ResponseMessage.Message("Picture not successfully uploaded"));
                }
            }
            return BadRequest(ResponseMessage.Message("Picture not uploaded"));
        }

        // Gets user by Id.
        [HttpGet]
        [Route("{Id}")]
        [Authorize(Roles = "Agent, Admin")]
        public async Task<IActionResult> Get(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id)) return BadRequest(ResponseMessage.Message("Invalid Id"));

            var user = await _userManager.FindByIdAsync(Id);


            if (user == null)
                return NotFound(ResponseMessage.Message("User not found"));

            if (!user.Active)
                return Unauthorized(ResponseMessage.Message("Not an active account"));


            // Returns the field agent by userId
            var agent = await _ctx.FieldAgents.FirstOrDefaultAsync(a => a.ApplicationUserId == Id);
            if (agent == null) return NotFound(ResponseMessage.Message("User's extended details not found"));

            //  Returns the bank account of that particular field agent using the fieldAgentID
            var bank = await _ctx.BankAccounts.FirstOrDefaultAsync(a => a.FieldAgentId == agent.FieldAgentId);
            if (bank == null) return NotFound(ResponseMessage.Message("User's bank details not found"));

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
                ApplicationUser user = null;
                try
                {
                    var authSupportService = new AuthSupportService(_userManager, _agentRepository, _bankRepository);
                    user = await authSupportService.verifyUser(Id);
                }
                catch (Exception e)
                {
                    return BadRequest(ResponseMessage.Message(e.Message));
                }


                // check if user with id is logged in
                var loggedInUserId = _userManager.GetUserId(User);
                if (loggedInUserId != Id)
                    return BadRequest(ResponseMessage.Message($"Id: {Id} does not match loggedIn user Id"));


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
                    return BadRequest(ResponseMessage.Message("Failed to update user"));
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
                    return BadRequest(ResponseMessage.Message("Failed to update additional details"));
                }

                return Ok(ResponseMessage.Message("Updated Successfully!"));

            }
            return BadRequest(ModelState);
        }


        //change pin
        [HttpPatch("{Id}/change-password")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> ChangePassword(string Id, [FromBody] ResetUserPwdDTO userToUpdate)
        {
            ApplicationUser user = null;
            try
            {
                var authSupportService = new AuthSupportService(_userManager, _agentRepository, _bankRepository);
                user = await authSupportService.verifyUser(Id);
            }catch(Exception e)
            {
                return BadRequest(ResponseMessage.Message(e.Message));
            }

            // check if user with id is logged in
            var loggedInUserId = _userManager.GetUserId(User);
            if (loggedInUserId != Id)
                return BadRequest(ResponseMessage.Message($"Id: {Id} does not match loggedIn user Id"));

            if (ModelState.IsValid)
            {

                var updatePwd = await _userManager.ChangePasswordAsync(user, userToUpdate.CurrentPwd, userToUpdate.NewPwd);

                if (updatePwd.Succeeded) return Ok(ResponseMessage.Message("Password Changed!"));

                foreach (var error in updatePwd.Errors)
                {
                    ModelState.AddModelError("", $"{error.Code} - {error.Description}");
                }
            }

            return BadRequest(ModelState);
        }


        //remove user
        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            if (Id == null)
                BadRequest(ResponseMessage.Message("You need to provide user Id"));

            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                NotFound($"User with id {Id} was not found");

            FieldAgent agent = null;
            try
            {
                // get field agent
                agent = await _agentRepository.GetAgentById(user.Id);
                if (agent == null)
                    throw new Exception($"Agent with user id {user.Id} was not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseMessage.Message("Failed to delete user!"));
            }

            List<BankAccount> bankDetails = new List<BankAccount>();
            try
            {
                // get bank details
                bankDetails = await _bankRepository.GetBankDetailsByAgent(agent.FieldAgentId);
                if (bankDetails == null)
                    throw new Exception($"Bank with field agent id {agent.FieldAgentId} was not found");

                foreach (var detail in bankDetails)
                {
                    if (!await _bankRepository.DdeleteBankDetail(detail))
                        throw new Exception("Could not delete bank record");
                }

                if (!await _agentRepository.DeleteAgent(agent))
                    throw new Exception("Could not delete agent record");

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError("", err.Description);
                    return BadRequest(ModelState);
                }

                if (!await _requestRepository.DeleteRequestByPhone(user.PhoneNumber))
                    throw new Exception("Could not delete request record");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseMessage.Message("Failed to delete user!"));
            }
            return Ok(ResponseMessage.Message("User deleted!"));
        }


    }
}
