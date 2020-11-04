using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ForgotPasswordDTO
    {
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [MaxLength(4, ErrorMessage = "Pin must not be morethan 4 digits")]
        public string OTPCode { get; set; }

        [Required]
        [MaxLength(4, ErrorMessage = "Pin must not be morethan 4 digits")]
        public string NewPin { get; set; }
    }
}
