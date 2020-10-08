using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class VerifiedUserDTO
    {
        public string UserId { get; set; }
        public string currentPin { get; set; }
        public string newPin { get; set; }
    }
}
