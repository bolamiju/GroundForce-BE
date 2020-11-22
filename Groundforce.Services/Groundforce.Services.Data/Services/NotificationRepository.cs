using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _ctx;
        public int TotalNotifications { get; set; }

        public NotificationRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> AddNotification(Notification model)
        {
            await _ctx.Notifications.AddAsync(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteNotification(Notification model)
        {
            _ctx.Notifications.Remove(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Notification>> GetAllNotifications()
        {
            var allNotifications = await _ctx.Notifications.ToListAsync();
            TotalNotifications = allNotifications.Count();
            return allNotifications;
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsPaginated(int page, int per_page)
        {
            var allNotifications = await GetAllNotifications();
            var paginatedNotifications = allNotifications.Skip((page - 1) * per_page).Take(per_page).ToList();
            return paginatedNotifications;
        }

        public async Task<Notification> GetNotificationById(string Id)
        {
            return await _ctx.Notifications.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<bool> UpdateNotification(Notification updateModel)
        {
            _ctx.Notifications.Update(updateModel);
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}