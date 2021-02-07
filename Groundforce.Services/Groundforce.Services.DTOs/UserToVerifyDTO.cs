using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class UserToVerifyDTO
    {
        [Required]
        public string BankCode { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "Account Number must not be more than 10 characters")]
        public string AccountNumber { get; set; }
        [MaxLength(14, ErrorMessage = "Additional Phone Number must not be more than 14 characters")]
        public string AdditionalPhoneNumber { get; set; }
        [Required]
        [MaxLength(1, ErrorMessage = "Gender must be 1 character")]
        public string Gender { get; set; }
    }
}
