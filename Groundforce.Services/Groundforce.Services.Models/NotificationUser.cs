﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.Models
{
    public class NotificationUser
    {
        [Key]
        public int Id { get; set; }
        public string NotificationId { get; set; }
        public Notification Notification { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
