﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Groundforce.Services.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Groundforce.Services.Models;
using Groundforce.Services.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Groundforce.Common.Utilities;
using Microsoft.Extensions.Options;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // private fields
        private readonly IConfiguration _config;

        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _ctx;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPhotoService _photoService;

        public AccountController(IConfiguration configuration, ILogger<AccountController> logger,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, AppDbContext ctx, IWebHostEnvironment webHostEnvironment,
             IPhotoService photoService)
        {
            _config = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _ctx = ctx;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _photoService = photoService;
        }

        // register user
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserToRegisterDTO model)
        {
            var userToAdd = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);

            if (userToAdd != null)
                return BadRequest("Email already exist");

            //create new applicationUser
            var user = new ApplicationUser
            {
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DOB = model.DOB,
                LGA = model.LGA,
                PlaceOfBirth = model.PlaceOfBirth,
                State = model.State,
                CreatedAt = DateTime.Now,
                Gender = model.Gender,
                HomeAddress = model.HomeAddress,
            };

            var result = await _userManager.CreateAsync(user, model.PIN);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }

                return BadRequest("Failed to create user!");
            }

            await _userManager.AddToRoleAsync(user, "Agent");

            var createdUser = await _userManager.FindByEmailAsync(model.Email);

            if (createdUser == null) return BadRequest();

            //create new field agent
            var agent = new FieldAgent
            {
                ApplicationUserId = createdUser.Id,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Religion = model.Religion,
                AdditionalPhoneNumber = model.AdditionalPhoneNumber
            };

            try
            {
                await _ctx.FieldAgents.AddAsync(agent);
                _ctx.SaveChanges();
            }
            catch (Exception e)
            {
                _ctx.Remove(createdUser);
                _ctx.SaveChanges();
                _logger.LogError(e.Message);
                return BadRequest("Failed to add additional details");
            }

            var createdFieldAgent = _ctx.FieldAgents.Where(x => x.ApplicationUserId == createdUser.Id).FirstOrDefault();

            if (createdFieldAgent == null) return BadRequest();

            var bank = new BankAccount
            {
                FieldAgentId = createdFieldAgent.FieldAgentId,
                BankName = model.BankName,
                AccountNumber = model.AccountNumber
            };

            try
            {
                await _ctx.BankAccounts.AddAsync(bank);
                _ctx.SaveChanges();
            }
            catch (Exception e)
            {
                _ctx.Remove(createdUser);
                _ctx.Remove(createdFieldAgent);
                _ctx.SaveChanges();
                _logger.LogError(e.Message);
                return BadRequest("Failed to add bank details");
            }

            return Ok();
        }

        //User Login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (ModelState.IsValid)
            {
                //get user by email
                var user = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);

                //Check if user exist
                if (user == null)
                {
                    return BadRequest("Account does not exist");
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Pin, false, false);

                if (result.Succeeded)
                {
                    var getToken = GetTokenHelperClass.GetToken(user, _config);
                    return Ok(getToken);
                }

                ModelState.AddModelError("", "Invalid creadentials");
                return Unauthorized(ModelState);
            }

            return BadRequest(model);
        }

        //change pin
        [HttpPatch]
        [Route("changePin")]
        public async Task<IActionResult> ChangePin([FromBody] ResetUserPwdDTO userToUpdate)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userToUpdate.UserId);

                if (user == null) return NotFound();

                var updatePwd =
                    await _userManager.ChangePasswordAsync(user, userToUpdate.CurrentPwd, userToUpdate.NewPwd);

                if (updatePwd.Succeeded) return Ok();

                foreach (var error in updatePwd.Errors)
                {
                    ModelState.AddModelError("", $"{error.Code} - {error.Description}");
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPatch("{userId}/picture")]
        public async Task<IActionResult> UploadPicture([FromForm] PhotoForCreation photoFile, string userId)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userId);  // 1.
                if (user == null)
                {
                    return BadRequest("User not found");
                }

                var file = photoFile.PhotoFile;                            // 2.
                var uploadResult = new ImageUploadResult();                // 3.

                if (file.Length > 0)                                        // 4.
                {
                    try
                    {
                        uploadResult = _photoService.Upload(file);  //  5.
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }

                    user.AvatarUrl = uploadResult.Url.ToString();
                    user.PublicId = uploadResult.PublicId;
                    await _userManager.UpdateAsync(user);

                    return Ok("Photo uploaded");
                }

                return BadRequest("No File Found");
            }

            return BadRequest(ModelState);
        }
    }
}