using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Groundforce.Services.Models
{
    public class VerificationItem
    {
        [Key]
        [Required]
        public string ItemId { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "Address must not be more than 250 characters")]
        [Display(Name = "Address")]
        public string ItemName { get; set; }

        [Required(ErrorMessage ="Required to provide whom address is added by")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Mission Mission { get; set; }

    }
}
