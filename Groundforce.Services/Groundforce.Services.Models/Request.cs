using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Groundforce.Services.Models
{
    public class Request
    {
        [Key]
        [Required]
        public string RequestId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public bool IsVerified { get; set; } = false;
        public bool IsBlock { get; set; } = false;
        public int RequestAttempt { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
