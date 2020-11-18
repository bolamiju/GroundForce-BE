using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Verification code must all be digits")]
        public string NewPin { get; set; }
    }
}