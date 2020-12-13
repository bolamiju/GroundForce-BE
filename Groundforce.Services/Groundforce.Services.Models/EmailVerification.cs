using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.Models
{
    public class EmailVerification
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(4)]
        public string VerificationCode { get; set; }

        public bool IsVerified { get; set; } = false;
    }
}
