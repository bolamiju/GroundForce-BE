using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Groundforce.Services.Models
{
    public class Notification
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Notifications { get; set; }
        [Required]
        public NotificationType Type { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;

        List<NotificationUser> NotificationUsers { get; set; }

        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }

        public Notification()
        {
            NotificationUsers = new List<NotificationUser>();
        }
    }
}