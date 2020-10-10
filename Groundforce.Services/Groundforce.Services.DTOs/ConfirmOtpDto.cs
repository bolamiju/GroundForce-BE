using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.API.DTOs
{
    public class ConfirmOtpDto
    {
        public string VerifyCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}
