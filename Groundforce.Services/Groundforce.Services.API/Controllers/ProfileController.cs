using Groundforce.Common.Utilities;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPhotoServices _cloudinaryServices;
        public ProfileController(ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment,
            IPhotoServices cloudinaryServices)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _cloudinaryServices = cloudinaryServices;
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
                    userToUpdate.AvatarUrl = _cloudinaryServices.UploadAvatar(picture);
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
    }
}
