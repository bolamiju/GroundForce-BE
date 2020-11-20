using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class UserToVerifyDTO
    {
        [Required]
        public IFormFile Photo { get; set; }
        [Required]
        public string BankCode { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string Religion { get; set; }
        public string AdditionalPhoneNumber { get; set; }
        [Required]
        public string Gender { get; set; }
    }
}
