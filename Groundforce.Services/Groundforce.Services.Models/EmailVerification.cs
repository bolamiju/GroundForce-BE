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
        public byte[] CodeHash { get; set; }

        [Required]
        public byte[] CodeSalt { get; set; }

        public bool IsVerified { get; set; } = false;
    }
}
