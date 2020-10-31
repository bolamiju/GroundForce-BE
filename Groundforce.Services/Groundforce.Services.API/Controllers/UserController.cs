
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
            IBankRepository bankRepository, IRequestRepository requestRepository, IAdminRepository adminRepository )
        {
            _userManager = userManager;
            _ctx = ctx;
            _cloudinaryConfig = cloudinaryConfig;
            _agentRepository = agentRepository;
            _bankRepository = bankRepository;
            _requestRepository = requestRepository;
            _adminRepository = adminRepository;
            _logger = logger;
        }

        //// register agent
        [HttpPost("register/agent")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAgent(UserToRegisterDTO model)
        {
            // ensure that number has gone through verification and confirmation
            var phoneNumberIsInRequestTable = await _requestRepository.GetRequestByPhone(model.PhoneNumber);
            if (phoneNumberIsInRequestTable == null)
                return BadRequest(ResponseMessage.Message("Phone number has not been verified yet"));

            if (!phoneNumberIsInRequestTable.IsConfirmed)
                return BadRequest(ResponseMessage.Message("Phone number has not been confrimed yet"));


            // check if email aready exists
            var emailToAdd = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);
            if (emailToAdd != null)
                return BadRequest(ResponseMessage.Message("Email already exist"));

            // check if number aready exists
            var numberToAdd = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == model.PhoneNumber);
            if (numberToAdd != null)
                return BadRequest(ResponseMessage.Message("Phone number already exist"));


            //Add new applicationUser
            var userModel = new UserWithoutDetailsDTO
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DOB = model.DOB,
                LGA = model.LGA,
                PhoneNumber = model.PhoneNumber,
                PlaceOfBirth = model.PlaceOfBirth,
                State = model.State,
                Gender = model.Gender,
                HomeAddress = model.HomeAddress,
                PIN = model.PIN
            };

            var authSupportService = new AuthSupportService(_userManager, _agentRepository, _bankRepository);
            var result = await authSupportService.CreateAppUser(userModel, "Agent");
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return BadRequest(ModelState);
            }


            //Add field agent
            ApplicationUser createdUser = await _userManager.FindByEmailAsync(model.Email);
            bool isAgentCreated = false;
            if (createdUser != null)
            {
                try
                {
                    isAgentCreated = await authSupportService.CreateFieldAgent(createdUser.Id, model);
                }
                catch (Exception e)
                {
                    await _userManager.DeleteAsync(createdUser);
                    await _userManager.RemoveFromRoleAsync(createdUser, "Agent");
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Failed to add additional details"));
                }
            }
            else { return BadRequest(ResponseMessage.Message("User not created")); }


            // Add bank details
            bool isBankCreated = false;
            FieldAgent newCreatedAgent = null;
            if (isAgentCreated)
            {
                try
                {
                    newCreatedAgent = await _agentRepository.GetAgentById(createdUser.Id);
                    isBankCreated = await authSupportService.CreateBankDetails(newCreatedAgent.FieldAgentId, model);
                }
                catch (Exception e)
                {
                    await _userManager.DeleteAsync(createdUser);
                    await _userManager.RemoveFromRoleAsync(createdUser, "Agent");
                    await _agentRepository.DeleteAgent(newCreatedAgent);
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Failed to add bank details"));
                }
            }
            else { return BadRequest(("Field agent not created")); }

            return Ok(ResponseMessage.Message(createdUser.Id));
        }

        //// register admin
        [HttpPost("register/admin")]
        [Authorize(Roles = "Admin, Agent")]
        public async Task<IActionResult> RegisterAdmin(UserWithoutDetailsDTO model)
        {
            // ensure that number has gone through verification and confirmation
            var phoneNumberIsInRequestTable = await _requestRepository.GetRequestByPhone(model.PhoneNumber);
            if (phoneNumberIsInRequestTable == null)
                return BadRequest(ResponseMessage.Message("Phone number has not been verified yet"));

            if (!phoneNumberIsInRequestTable.IsConfirmed)
                return BadRequest(ResponseMessage.Message("Phone number has not been confrimed yet"));


            // check if email aready exists
            var emailToAdd = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);
            if (emailToAdd != null)
                return BadRequest(ResponseMessage.Message("Email already exist"));

            // check if number aready exists
            var numberToAdd = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == model.PhoneNumber);
            if (numberToAdd != null)
                return BadRequest(ResponseMessage.Message("Phone number already exist"));



            //Add new applicationUser
            var authSupportService = new AuthSupportService(_userManager, _agentRepository, _bankRepository);
            var result = await authSupportService.CreateAppUser(model, "Admin");
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return BadRequest(ModelState);
            }


            //Add admin
            ApplicationUser createdUser = await _userManager.FindByEmailAsync(model.Email);
            bool isAdminCreated = false;
            if (createdUser != null)
            {
                string adminId = "";
                Admin admin = null;
                do
                {
                    adminId = Guid.NewGuid().ToString();
                    admin = await _adminRepository.GetAdminById(adminId);
                } while (admin != null);

                try
                {
                    var adminToAdd = new Admin
                    {
                        AdminId = adminId,
                        ApplicationUserId = createdUser.Id
                    };

                    isAdminCreated = await _adminRepository.AddAdmin(adminToAdd);
                }
                catch (Exception e)
                {
                    await _userManager.DeleteAsync(createdUser);
                    await _userManager.RemoveFromRoleAsync(createdUser, "Admin");
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Failed to add additional details"));
                }
            }
            else { return BadRequest(ResponseMessage.Message("User not created")); }

            return Ok(ResponseMessage.Message(createdUser.Id));
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
                return BadRequest(ResponseMessage.Message("User does not exist"));
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
            if (string.IsNullOrWhiteSpace(Id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(Id);


            if (user == null)
            {

                return NotFound(ResponseMessage.Message("User not found"));
            }

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
                var user = await _userManager.FindByIdAsync(Id);
                if (user == null) return BadRequest(ResponseMessage.Message("User Does Not Exist"));
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
                    return BadRequest(ResponseMessage.Message("Failed to update user!"));
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

                return Ok("Success");

            }
            return BadRequest(ModelState);
        }


        //change pin
        [HttpPatch("{Id}/ChangePassword")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> ChangePassword(string Id, [FromBody] ResetUserPwdDTO userToUpdate)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null) return NotFound();

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
                if(agent == null)
                    throw new Exception($"Agent with user id {user.Id} was not found");
            }
            catch(Exception ex)
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

                foreach(var detail in bankDetails)
                {
                    if(!await _bankRepository.DdeleteBankDetail(detail))
                        throw new Exception("Could not delete bank record");
                }

                if(!await _agentRepository.DeleteAgent(agent))
                    throw new Exception("Could not delete agent record");

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError("", err.Description);
                    return BadRequest(ModelState);
                }

                if(!await _requestRepository.DeleteRequestByPhone(user.PhoneNumber))
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
