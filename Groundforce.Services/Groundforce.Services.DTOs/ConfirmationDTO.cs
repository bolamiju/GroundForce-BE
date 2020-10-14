using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ConfirmationDTO
    {
        public string PhoneNumber { get; set; }
        public string VerifyCode { get; set; }
    }
}