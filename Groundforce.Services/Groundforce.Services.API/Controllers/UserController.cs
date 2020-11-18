
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
                if (string.IsNullOrWhiteSpace(id)) return BadRequest(ResponseMessage.Message("Bad request",errors: "Invalid Id"));

                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return NotFound(ResponseMessage.Message("Notfound", errors: $"User with id: {id} was not found"));

                if (_userManager.GetUserId(User) != id )
                    return Unauthorized(ResponseMessage.Message("Unauthorized", errors: $"Id {id} does not match for loggedin user"));

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
                    if (agent == null) return NotFound(ResponseMessage.Message("Notfound", errors: "User's extended details not found"));

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
                return BadRequest(ResponseMessage.Message("Bad request", errors: "Data processing error"));
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
                    return NotFound(ResponseMessage.Message("Notfound", errors: $"User with id: {model.Id} was not found"));

                if (_userManager.GetUserId(User) != model.Id)
                    return Unauthorized(ResponseMessage.Message("Unauthorized", errors: $"Id {model.Id} does not match for loggedin user"));


                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.DOB = model.DOB;
                user.Gender = model.Gender;


                // update user extended details if user is an agent
                if (await _userManager.IsInRoleAsync(user, "agent"))
                {
                    var agent = await _agentRepository.GetAgentById(model.Id);
                    if (agent == null) return NotFound(ResponseMessage.Message("Notfound", errors: "User's extended details not found"));
                    
                    agent.AdditionalPhoneNumber = model.AdditionalPhoneNumber;
                    agent.Religion = model.Religion;

                    var res = await _agentRepository.UpdateAgent(agent);
                    if(!res)
                        return NotFound(ResponseMessage.Message("Bad request", errors: "Failed to update user's extended details"));
                }

                // update user
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return BadRequest(ResponseMessage.Message("Bad request", errors: "Failed to update user"));
                }

                return Ok(ResponseMessage.Message("Success", data: "Updated Successfully!"));

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
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: "Data processing error"));
            }

            // check if user with id is logged in
            var loggedInUserId = _userManager.GetUserId(User);
            if (loggedInUserId != Picture.Id)
                return BadRequest(ResponseMessage.Message($"Id: {Picture.Id} does not match loggedIn user Id"));

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
                return BadRequest(ResponseMessage.Message("Bad request", e.Message));
            }
              
        }


    }
}
