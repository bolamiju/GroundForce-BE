using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class MissionToEditDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Verification Id")]
        public string VerificationItemId { get; set; }

        [Required]
        [Display(Name = "Field agent Id")]
        public string FieldAgentId { get; set; }
    }
}
