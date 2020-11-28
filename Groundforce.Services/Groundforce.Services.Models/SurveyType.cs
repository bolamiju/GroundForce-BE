using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.Models
{
    public class SurveyType
    {
        [Key]
        [Required]
        public string SurveyTypeId { get; set; }
        [Required]
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<Survey> Surveys { get; set; }
    }
}
