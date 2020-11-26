using System;
using System.Collections.Generic;
using System.Linq;
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
        [HttpPost("{AdminId}")]
        public async Task<IActionResult> CreateNotification(string AdminId, [FromBody] NotificationDTO newNotification)
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

                var currentUser = _userManager.Users.FirstOrDefault(x => x.Id == AdminId);
                var createNotification = new Notification
                {
                    Id = NotifID,
                    Notifications = newNotification.Description,
                    Type = newNotification.Type,
                    ApplicationUser = currentUser
                };

                var addNotification = await _notificationRepository.AddNotification(createNotification);
                if (addNotification)
                {
                    return Ok(ResponseMessage.Message("Success", data: $"Notification with id {NotifID} has been created"));
                }
                return BadRequest(ResponseMessage.Message("Bad Request", errors: $"Failed to create notification"));
            }
            return BadRequest(ResponseMessage.Message("Bad Request", errors: $"Please enter the correct details"));
        }

        //delete a notification
        [HttpDelete("{NotificationId}/delete-notification")]
        public async Task<IActionResult> DeleteNotification(string NotificationId)
        {
            if (NotificationId == null) BadRequest(ResponseMessage.Message("Bad Request", errors: "You need to provide a notification Id"));

            var fetchedNotification = await _notificationRepository.GetNotificationById(NotificationId);
            if (fetchedNotification != null)
            {
                var notificationToDelete = await _notificationRepository.DeleteNotification(fetchedNotification);

                if (notificationToDelete) return Ok(ResponseMessage.Message("Success", data: $"The {fetchedNotification.Type} notification with id {NotificationId} has been deleted"));

                return BadRequest(ResponseMessage.Message("Bad Request", errors: $"Could not delete the {fetchedNotification.Type} notification with id {NotificationId}. Please try again"));
            }
            return BadRequest(ResponseMessage.Message("Bad Request", errors: $"Notification with id {NotificationId} does not exists"));
        }

        //update a notification
        [HttpPatch("{Id}/edit-notification")]
        public async Task<IActionResult> UpdateNotification(string Id, [FromBody] NotificationDTO UpdateNotification)
        {
            if (Id == null) BadRequest(ResponseMessage.Message("Bad Request", errors: "You need to provide a notification Id"));

            var fetchedNotification = await _notificationRepository.GetNotificationById(Id);
            if (fetchedNotification != null)
            {
                fetchedNotification.Notifications = UpdateNotification.Description;
                fetchedNotification.Type = UpdateNotification.Type;
                fetchedNotification.DateUpdated = DateTime.Now;

                var notificationToUpdate = await _notificationRepository.UpdateNotification(fetchedNotification);

                if (notificationToUpdate) return Ok(ResponseMessage.Message("Success", data: $"Notification with id {Id} has been updated"));

                return BadRequest(ResponseMessage.Message("Bad Request", errors: $"Could not update notification with id {Id}. Please try again"));
            }
            return BadRequest(ResponseMessage.Message("Bad Request", errors: $"Notification with id {Id} was not found"));
        }

        //get all notifications
        [HttpGet]
        [Route("all-notifications")]
        public async Task<IActionResult> FetchAllNotifications()
        {
            IEnumerable<Notification> result;
            var notificationList = new List<NotificationToReturnDTO>();

            try
            {
                result = await _notificationRepository.GetAllNotifications();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad Request", errors: "Could not fetch notifications"));
            }

            if (result.Count() == 0)
                return Ok(ResponseMessage.Message("Success", data: $"There are no notifications"));

            foreach (var notification in result)
            {
                var notificationDTOResult = new NotificationToReturnDTO
                {
                    Id = notification.Id,
                    Notifications = notification.Notifications,
                    Type = notification.Type.ToString(),
                    ApplicationUserId = notification.ApplicationUserId
                };
                notificationList.Add(notificationDTOResult);
            }
            return Ok(ResponseMessage.Message($"Success", data: notificationList));
        }

        //get a notification by Id
        [HttpGet]
        [Route("{NotificationId}")]
        public async Task<IActionResult> FetchSingleNotification(string NotificationId)
        {
            if (NotificationId == null) BadRequest(ResponseMessage.Message("Bad Request", errors: "You need to provide a notification Id"));

            var notificationWithId = await _notificationRepository.GetNotificationById(NotificationId);
            if (notificationWithId == null)
                return BadRequest(ResponseMessage.Message("Bad Request", errors: $"No such notification with id {NotificationId} exists"));

            var notificationDTOResult = new NotificationToReturnDTO
            {
                Id = notificationWithId.Id,
                Notifications = notificationWithId.Notifications,
                Type = notificationWithId.Type.ToString(),
                ApplicationUserId = notificationWithId.ApplicationUserId
            };
            return Ok(ResponseMessage.Message("Success", data: notificationDTOResult));
        }

        //get all notifications paginated
        [HttpGet]
        [Route("{page}/all-notifications")]
        public async Task<IActionResult> FetchNotificationsPaginated(int page)
        {
            IEnumerable<Notification> paginatedResults;
            var notificationList = new List<NotificationToReturnDTO>();

            paginatedResults = await _notificationRepository.GetAllNotificationsPaginated(page, per_page);
            if (paginatedResults == null)
                return BadRequest(ResponseMessage.Message("Bad Request", errors: "There are no notifications"));

            foreach (var notification in paginatedResults)
            {
                var notificationDTOResult = new NotificationToReturnDTO
                {
                    Id = notification.Id,
                    Notifications = notification.Notifications,
                    Type = notification.Type.ToString(),
                    ApplicationUserId = notification.ApplicationUserId
                };
                notificationList.Add(notificationDTOResult);
            }
            return Ok(ResponseMessage.Message("Success", data: notificationList));
        }
    }
}