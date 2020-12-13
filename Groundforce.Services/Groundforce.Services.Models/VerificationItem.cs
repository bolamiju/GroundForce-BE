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
        [Column("AddedBy")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string UpdatedBy { get; set; }

        [Required]
        [MaxLength(20)]
        public string Title { get; set; }

        [Required]
        [MaxLength(250)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Mission Mission { get; set; }

    }
}
