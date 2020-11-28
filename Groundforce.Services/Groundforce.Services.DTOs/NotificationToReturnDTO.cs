using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class NotificationToReturnDTO
    {
        public string Id { get; set; }
        public string Notifications { get; set; }
        public string Type { get; set; }
        public string ApplicationUserId { get; set; }
    }
}