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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsBlocked { get; set; } = false;
        public int RequestAttempt { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
