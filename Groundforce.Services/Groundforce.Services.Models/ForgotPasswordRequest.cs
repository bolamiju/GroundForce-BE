using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.Models
{
    public class ForgotPasswordRequest
    {
        public string ToEmail { get; set; }
        public string ResetPasswordLink { get; set; }
    }
}
