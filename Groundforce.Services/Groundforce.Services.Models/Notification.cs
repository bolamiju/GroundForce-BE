using System;
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

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}