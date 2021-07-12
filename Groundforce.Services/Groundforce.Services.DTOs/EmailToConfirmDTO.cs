using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class EmailToConfirmDTO
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }


        [StringLength(4, ErrorMessage = "Verification code must be exactly 4 digits", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Verification code must all be digits")]
        public string VerificationCode { get; set; }
    }
}
