using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.Models
{
    public class PasswordVerification
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
