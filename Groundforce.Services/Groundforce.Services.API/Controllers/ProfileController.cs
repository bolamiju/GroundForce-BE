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

namespace Groundforce.Services.API.Controllers
{
    //[Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PhotoService _photoService;

        public ProfileController(ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _logger = logger;
            _userManager = userManager;
            _photoService = new PhotoService(cloudinaryConfig);
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
                    await _userManager.UpdateAsync(user);

                    return Ok("Photo uploaded");
                }

                return BadRequest("No File Found");
            }

            return BadRequest(ModelState);
        }
    }
}