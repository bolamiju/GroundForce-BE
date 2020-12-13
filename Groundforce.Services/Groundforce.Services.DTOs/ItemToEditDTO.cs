using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ItemToEditDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "Title must not be more than 20 characters")]
        public string Title { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "Description must not be more than 250 characters")]
        public string Description { get; set; }
    }
}
