using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ChangePwdDTO
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Verification code must all be digits")]
        public string CurrentPassword { get; set; }
        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Verification code must all be digits")]
        public string NewPassword { get; set; }
    }
}
