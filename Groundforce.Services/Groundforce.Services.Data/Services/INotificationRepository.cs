using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface INotificationRepository
    {
        int TotalNotifications { get; set; }
        //Task<List<Notification>> GetNotificationsByUserId(string userId, int page, int per_page);
        Task<Notification> GetNotificationById(string Id);
        Task<IEnumerable<Notification>> GetAllNotificationsPaginated(int page, int per_page);
        Task<bool> AddNotification(Notification model);
        Task<bool> UpdateNotification(Notification updateModel);
        Task<bool> DeleteNotification(Notification model);
    }
}