using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Groundforce.Services.Models
{
    public class UserActivity
    {
        [Key]
        [Required]
        public string Id { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string RecordId { get; set; }

        [MaxLength(20)]
        public string Description { get; set; }

    }
}
