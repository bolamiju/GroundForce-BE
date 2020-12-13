using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class OTPToConfirmDTO
    {
        [Required]
        [RegularExpression(@"^\+\d{3}\d{9,10}$", ErrorMessage = "Must have country-code and must be 13, 14 chars long e.g. +2348050000000")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(4, ErrorMessage = "Pin must be exactly 4 digits", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Only digit allowed")]
        public string VerifyCode { get; set; }
    }
}