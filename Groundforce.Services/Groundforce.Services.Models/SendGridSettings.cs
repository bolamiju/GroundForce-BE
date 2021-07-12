using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.Models
{
    public class SendGridSettings
    {
        public string APIKey { get; set; }
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string GroundForceLogo { get; set; }
    }
}
