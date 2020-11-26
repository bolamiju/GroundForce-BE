
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
using Microsoft.Extensions.Configuration;

namespace Groundforce.Services.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IAgentRepository _agentRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IPhotoRepository _photoRepo;

        public UserController(ILogger<UserController> logger, UserManager<ApplicationUser> userManager, 
            IOptions<CloudinarySettings> cloudinaryConfig, IAgentRepository agentRepository,
                                 IRequestRepository requestRepository,
                                 IPhotoRepository photoRepository)
        {
            _userManager = userManager;
            _cloudinaryConfig = cloudinaryConfig;
            _agentRepository = agentRepository;
            _logger = logger;
            _requestRepository = requestRepository;
            _photoRepo = photoRepository;
        }
        

        // Gets user by Id.
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                // validate and ensure that the user is an active user
                if (string.IsNullOrWhiteSpace(id)) return BadRequest(ResponseMessage.Message("Bad request",errors: new { message = "Invalid Id" }));

                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return NotFound(ResponseMessage.Message("Notfound", errors: new { message = $"User with id: {id} was not found" }));

                if (_userManager.GetUserId(User) != id && !User.IsInRole("Admin"))
                    return Unauthorized(ResponseMessage.Message("Unauthorized", errors: new { message = $"User must be logged-in or must have admin role" }));

                // construct the object
                var appUser = new 
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.DOB,
                    user.Gender,
                    user.Email,
                    user.AvatarUrl,
                    user.PublicId,
                    user.UpdatedAt,
                };

                // Returns the field agent by userId
                //var userRoles = await _userManager.GetRolesAsync(user);
                if (await _userManager.IsInRoleAsync(user,"agent"))
                {
                    var agent = await _agentRepository.GetAgentById(id);
                    if (agent == null) return NotFound(ResponseMessage.Message("Notfound", errors: new { message = "User's extended details not found" }));

                    var profile = new UserToReturnDTO
                    {
                        Id = appUser.Id,
                        FirstName = appUser.FirstName,
                        LastName = appUser.LastName,
                        DOB = appUser.DOB,
                        Gender = appUser.Gender,
                        Religion = agent.Religion,
                        Email = appUser.Email,
                        AdditionalPhoneNumber = agent.AdditionalPhoneNumber,
                        ResidentialAddress = agent.ResidentialAddress,
                        BankName = agent.AccountName,
                        AccountNumber = agent.AccountNumber,
                        AvatarUrl = appUser.AvatarUrl,
                        PublicId = appUser.PublicId
                    };
                    return Ok(ResponseMessage.Message("User found", data: profile));
                }

                return Ok(ResponseMessage.Message("User found", data: appUser));
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Data processing error" }));
            }
        }


        // edit field agent
        [HttpPut]
        public async Task<IActionResult> EditUser([FromBody] UserToEditDTO model)
        {
            if (ModelState.IsValid)
            {
                // validate and ensure that the user is an active user
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user == null)
                    return NotFound(ResponseMessage.Message("Notfound", errors: new { message = $"User with id: {model.Id} was not found" }));

                if (_userManager.GetUserId(User) != model.Id)
                    return Unauthorized(ResponseMessage.Message("Unauthorized", errors: new { message = $"User must be logged-in" }));


                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.DOB = model.DOB;
                user.Gender = model.Gender;


                // update user extended details if user is an agent
                if (await _userManager.IsInRoleAsync(user, "agent"))
                {
                    var agent = await _agentRepository.GetAgentById(model.Id);
                    if (agent == null) return NotFound(ResponseMessage.Message("Notfound", errors: new { message = "User's extended details not found" }));
                    
                    agent.AdditionalPhoneNumber = model.AdditionalPhoneNumber;
                    agent.Religion = model.Religion;

                    var res = await _agentRepository.UpdateAgent(agent);
                    if(!res)
                        return NotFound(ResponseMessage.Message("Bad request", errors: new { message = "Failed to update user's extended details" }));
                }

                // update user
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to update user" }));
                }

                return Ok(ResponseMessage.Message("Success", data: new { message = "Updated Successfully!" }));

            }
            return BadRequest(ModelState);
        }


        //updates user picture
        [HttpPatch]
        [Route("picture")]
        public async Task<IActionResult> UpdatePicture([FromForm] PhotoToUploadDTO Picture)
        {
            ApplicationUser user = null;
            try
            {
                var authSupportService = new AuthSupportService(_userManager, _agentRepository);
                user = await _userManager.FindByIdAsync(Picture.Id);
                if(user == null)
                    return BadRequest(ResponseMessage.Message("Notfound", errors: new { message = $"User with Id: {Picture.Id} was not found" }));

                // check if user with id is logged in
                if (_userManager.GetUserId(User) != Picture.Id)
                    return Unauthorized(ResponseMessage.Message("Unauthorized", errors: new { message = $"User must be logged-in" }));

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Data processing error" }));
            }

            try
            {
                var uplResult = _photoRepo.UploadPix(Picture.Photo);
                user.AvatarUrl = uplResult.Url.ToString();
                user.PublicId = uplResult.PublicId;
                await _userManager.UpdateAsync(user);

                return Ok(ResponseMessage.Message("Picture upload was successful!", data: new { user.AvatarUrl, user.PublicId }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = e.Message }));
            }
              
        }


        //change pin
        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePwdDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = ModelState }));

            ApplicationUser user = null;
            try
            {
                // check if user with id is logged in
                var loggedInUserId = _userManager.GetUserId(User);
                if (loggedInUserId != model.UserId)
                    return BadRequest(ResponseMessage.Message("Bad request",errors: new { message = $"Id: {model.UserId} does not match loggedIn user Id" }));

                // get user
                user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                    return BadRequest(ResponseMessage.Message("Notfound", errors: new { message = $"User with Id: {model.UserId} was not found" }));

                // change password
                var updatePwd = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (updatePwd.Succeeded) return Ok(ResponseMessage.Message("Success", data: new { message = "Password Changed!" }));

                foreach (var error in updatePwd.Errors)
                {
                    ModelState.AddModelError("", $"{error.Code} - {error.Description}");
                }
                return BadRequest(ResponseMessage.Message("Bad request",errors: new { message = ModelState }));

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Data processing error" }));
            }

        }


        //verify user
        [HttpPatch("verify-account")]
        public async Task<IActionResult> VerifyUserAccount([FromBody] UserToVerifyDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user.IsVerified)
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "User is already verified" }));

                if (string.IsNullOrWhiteSpace(user.AvatarUrl))
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Photo must be uploaded" }));

                var agent = await _agentRepository.GetAgentById(user.Id);

                var validateAccountNumber = InputValidator.NUBANAccountValidator(model.BankCode, model.AccountNumber);

                if (!validateAccountNumber) return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Invalid account number" }));

                var accountName = Enum.GetName(typeof(BankCode), Convert.ToInt32(model.BankCode));

                agent.AccountName = accountName;
                agent.AccountNumber = model.AccountNumber;
                agent.Religion = model.Religion;
                agent.AdditionalPhoneNumber = model.AdditionalPhoneNumber;
                user.Gender = model.Gender;
                user.IsVerified = true;

                if (!await _agentRepository.UpdateAgent(agent))
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to update user" }));

                var update = await _userManager.UpdateAsync(user);
                if (!update.Succeeded)
                {
                    foreach (var err in update.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to update user" }));
                }

                return Ok(ResponseMessage.Message("Success", data: new { message = "Updated Successfully!" }));
            }
            return BadRequest(ResponseMessage.Message("Invalid model state", errors: new { message = ModelState }));
        }


        //remove user
        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            if (String.IsNullOrWhiteSpace(Id))
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Invalid Id" }));

            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                return NotFound(ResponseMessage.Message("Notfound", new { message = $"User with id {Id} was not found" }));

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
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Could not access data" }));
            }

            try
            {
                if (!await _agentRepository.DeleteAgent(agent))
                    throw new Exception("Could not delete agent record");

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError("", err.Description);
                    return BadRequest(ResponseMessage.Message("", errors: new { message = ModelState }));
                }

                if (!await _requestRepository.DeleteRequestByPhone(user.PhoneNumber))
                    throw new Exception("Could not delete request record");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ResponseMessage.Message("", errors: new { message = "Failed to delete user!" }));
            }
            return Ok(ResponseMessage.Message("Deleted successfully", data: new { message = "User deleted!" }));
        }



    }
}
