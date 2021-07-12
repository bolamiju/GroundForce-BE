using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.Models
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string MainHeader { get; set; }
        public string SubHeader { get; set; }
        public bool IsHidden { get; set; }
        public string Content { get; set; }
        public string ButtonName { get; set; }
        public string Link { get; set; }
        public string GroundForceUrl { get; set; }
    }
}
