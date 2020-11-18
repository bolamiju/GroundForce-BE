using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.Models
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Code { get; set; }
    }
}
