using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.Models
{
    public enum NotificationType
    {
        Broadcast, //admin form - 0
        Payment, //event driven - 1
        Task //event driven - 2
    }
}