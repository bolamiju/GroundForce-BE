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
        [Authorize(Roles = "admin")]
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

                var currentUser = await _userManager.GetUserAsync(User);
                var createNotification = new Notification
                {
                    Id = NotifID,
                    Notifications = newNotification.Description,
                    Type = newNotification.Type,
                    AddedBy = currentUser.Id,
                    UpdatedBy = currentUser.Id
                };

                var addNotification = await _notificationRepository.AddNotification(createNotification);

                if (addNotification) return Ok(ResponseMessage.Message("Success", data: new { message = $"Notification with id {NotifID} has been created" }));

                return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "Failed to create notification" }));
            }
            return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "Please enter the correct details" }));
        }

        //delete a notification
        [HttpDelete("{NotificationId}/delete-notification")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteNotification(string NotificationId)
        {
            if (String.IsNullOrWhiteSpace(NotificationId)) return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "You need to provide a notification Id" }));

            var fetchedNotification = await _notificationRepository.GetNotificationById(NotificationId);
            if (fetchedNotification != null)
            {
                var notificationToDelete = await _notificationRepository.DeleteNotification(fetchedNotification);

                if (notificationToDelete) return Ok(ResponseMessage.Message("Success", data: new { message = $"The {fetchedNotification.Type} notification with id {NotificationId} has been deleted" }));

                return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = $"Could not delete the {fetchedNotification.Type} notification with id {NotificationId}. Please try again" }));
            }
            return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = $"Notification with id {NotificationId} does not exists" }));
        }

        //update a notification
        [HttpPatch("{Id}/edit-notification")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateNotification(string Id, [FromBody] NotificationDTO UpdateNotification)
        {
            if (String.IsNullOrWhiteSpace(Id)) return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "You need to provide a notification Id" }));

            var currentAdmin = await _userManager.GetUserAsync(User);

            var fetchedNotification = await _notificationRepository.GetNotificationById(Id);
            if (fetchedNotification != null)
            {
                fetchedNotification.Notifications = UpdateNotification.Description;
                fetchedNotification.Type = UpdateNotification.Type;
                fetchedNotification.DateUpdated = DateTime.Now;
                fetchedNotification.UpdatedBy = currentAdmin.Id;

                var notificationToUpdate = await _notificationRepository.UpdateNotification(fetchedNotification);

                if (notificationToUpdate) return Ok(ResponseMessage.Message("Success", data: new { message = $"Notification with id {Id} has been updated" }));

                return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = $"Could not update notification with id {Id}. Please try again" }));
            }
            return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = $"Notification with id {Id} was not found" }));
        }

        //get a notification by Id
        [HttpGet]
        [Route("{NotificationId}")]
        [Authorize(Roles = "admin, client, agent")]
        public async Task<IActionResult> FetchSingleNotification(string NotificationId)
        {
            if (String.IsNullOrWhiteSpace(NotificationId))
                return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "You need to provide a notification Id" }));

            var notificationWithId = await _notificationRepository.GetNotificationById(NotificationId);
            if (notificationWithId == null)
                return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = $"No such notification with id {NotificationId} exists" }));

            var notificationDTOResult = new NotificationToReturnDTO
            {
                Id = notificationWithId.Id,
                Notifications = notificationWithId.Notifications,
                Type = notificationWithId.Type.ToString(),
                AddedBy = notificationWithId.AddedBy,
                UpdatedBy = notificationWithId.UpdatedBy,
                Date = notificationWithId.DateUpdated
            };
            return Ok(ResponseMessage.Message("Success", data: notificationDTOResult));
        }

        //get all notifications paginated
        [HttpGet]
        [Route("{page}/all-notifications")]
        [Authorize(Roles = "admin, client, agent")]
        public async Task<IActionResult> FetchNotificationsPaginated(int page)
        {
            IEnumerable<Notification> paginatedResults;
            var notificationList = new List<NotificationToReturnDTO>();

            paginatedResults = await _notificationRepository.GetAllNotificationsPaginated(page, per_page);
            if (paginatedResults == null)
                return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "There are no notifications" }));

            foreach (var notification in paginatedResults)
            {
                var notificationDTOResult = new NotificationToReturnDTO
                {
                    Id = notification.Id,
                    Notifications = notification.Notifications,
                    Type = notification.Type.ToString(),
                    AddedBy = notification.AddedBy,
                    UpdatedBy = notification.UpdatedBy,
                    Date = notification.DateUpdated
                };
                notificationList.Add(notificationDTOResult);
            }

            // set default page to start from 1 if page inputted is <= 0
            //****************************************************************************************/
            page = page <= 0 ? 1 : page;
            //****************************************************************************************/

            var paginatedNotifications = new PaginatedItemsToReturnDTO
            {
                PageMetaData = Util.Paginate(page, per_page, _notificationRepository.TotalNotifications),
                Data = notificationList
            };

            return Ok(ResponseMessage.Message("Success", data: paginatedNotifications));
        }

        //get all notifications by userId paginated
        //[HttpGet]
        //[Authorize(Roles = "admin, agent, client")]
        //[Route("{userId}/notifications/{page}")]
        //public async Task<IActionResult> FetchNotificationsByUserId(string userId, int page)
        //{
        //    IEnumerable<Notification> paginatedResults;
        //    var notificationList = new List<NotificationToReturnDTO>();

        //    paginatedResults = await _notificationRepository.GetNotificationsByUserId(userId, page, per_page);
        //    if (paginatedResults == null)
        //        return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "There are no notifications" }));

        //    foreach (var notification in paginatedResults)
        //    {
        //        var notificationDTOResult = new NotificationToReturnDTO
        //        {
        //            Id = notification.Id,
        //            Notifications = notification.Notifications,
        //            Type = notification.Type.ToString(),
        //            ApplicationUserId = notification.ApplicationUserId
        //        };
        //        notificationList.Add(notificationDTOResult);
        //    }
        //    return Ok(ResponseMessage.Message("Success", data: notificationList));
        //}
    }
}