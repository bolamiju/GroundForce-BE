using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class NotificationDTO
    {
        public string Description { get; set; }
        public NotificationType Type { get; set; }
    }
}