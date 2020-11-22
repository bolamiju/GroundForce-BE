using System;
using System.Threading.Tasks;
using Groundforce.Common.Utilities.Helpers;
using Groundforce.Services.Data;
using Groundforce.Services.Data.Services;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class NotificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationRepository _notificationRepository;
        private readonly AppDbContext _ctx;
        private readonly int per_page = 10;

        public NotificationController(IConfiguration configuration, ILogger<NotificationController> logger,
                                 SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager, AppDbContext ctx,
                                 INotificationRepository notificationRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _ctx = ctx;
            _notificationRepository = notificationRepository;
        }

        //create a notification
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] NotificationDTO newNotification)
        {
            if (ModelState.IsValid)
            {
                Notification result = null;
                string NotifID = "";
                do
                {
                    NotifID = Guid.NewGuid().ToString();
                    result = await _notificationRepository.GetNotificationById(NotifID);
                } while (result != null);

                var createNotification = new Notification
                {
                    Id = NotifID,
                    Notifications = newNotification.Description,
                    Type = newNotification.Type
                };

                var addNotification = await _notificationRepository.AddNotification(createNotification);
                if (addNotification)
                {
                    return Ok(ResponseMessage.Message($"Notification with id {NotifID} has been created"));
                }
                return BadRequest(ResponseMessage.Message($"Failed to create notification"));
            }
            return BadRequest(ResponseMessage.Message($"Please enter the correct details"));
        }

        //delete a notification
        [HttpDelete]
        public async Task<IActionResult> DeleteNotification([FromBody] string NotificationId)
        {
            if (NotificationId == null) BadRequest(ResponseMessage.Message("You need to provide a notification Id"));

            var fetch = await _notificationRepository.GetNotificationById(NotificationId);
            if (fetch != null)
            {
                var notificationToDelete = await _notificationRepository.DeleteNotification(fetch);

                if (notificationToDelete) return Ok(ResponseMessage.Message($"The {fetch.Type} notification with id {NotificationId} has been deleted"));

                return BadRequest(ResponseMessage.Message($"Could not delete the {fetch.Type} notification with id {NotificationId}. Please try again"));
            }
            return BadRequest(ResponseMessage.Message($"Notification with id {NotificationId} does not exists"));
        }

        //update a notification
        [HttpPatch]
        public async Task<IActionResult> UpdateNotification(string Id, [FromBody] NotificationDTO UpdateNotification)
        {
            if (Id == null) BadRequest(ResponseMessage.Message("You need to provide a notification Id"));

            var fetch = await _notificationRepository.GetNotificationById(Id);
            if (fetch != null)
            {
                fetch.Notifications = UpdateNotification.Description;
                fetch.Type = UpdateNotification.Type;

                var notificationToUpdate = await _notificationRepository.UpdateNotification(fetch);

                if (notificationToUpdate) return Ok(ResponseMessage.Message($"Notification with id {Id} has been updated"));

                return BadRequest(ResponseMessage.Message($"Could not update notification with id {Id}. Please try again"));
            }
            return BadRequest(ResponseMessage.Message($"Notification with id {Id} was not found"));
        }

        //get all notifications
        [HttpGet]
        [Route("/allNotifications")]
        public async Task<IActionResult> FetchAllNotifications()
        {
            var allNotifications = await _notificationRepository.GetAllNotifications();
            if (allNotifications != null)
            {
                return Ok(ResponseMessage.Message($"Successfully retrieved all notifications", allNotifications));
            }
            return BadRequest(ResponseMessage.Message($"There are no notifications"));
        }

        //get a notification by Id
        [HttpGet]
        [Route("{Id}")]
        public async Task<IActionResult> FetchSingleNotification([FromBody] string NotificationId)
        {
            if (NotificationId == null) BadRequest(ResponseMessage.Message("You need to provide a notification Id"));

            var notificationWithId = await _notificationRepository.GetNotificationById(NotificationId);
            if (notificationWithId != null)
            {
                return Ok(ResponseMessage.Message($"The notification with id {NotificationId} has been fetched", NotificationId));
            }
            return BadRequest(ResponseMessage.Message($"No such notification with id {NotificationId} exists"));
        }

        //get all notifications paginated
        [HttpGet]
        [Route("/paginated")]
        public async Task<IActionResult> FetchNotificationsPaginated(int page)
        {
            var paginatedResults = await _notificationRepository.GetAllNotificationsPaginated(page, per_page);
            if (paginatedResults != null)
            {
                return Ok(ResponseMessage.Message($"Successfully retrieved notifications", paginatedResults));
            }
            return BadRequest(ResponseMessage.Message("There are no notifications"));
        }
    }
}