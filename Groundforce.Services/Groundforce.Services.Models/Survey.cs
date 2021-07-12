using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.Models
{
    public class Survey
    {
        [Key]
        [Required]
        public string SurveyId { get; set; }
        [Required]
        public string Topic { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }

        [Required]
        [Display(Name = "Survey Type Id")]
        public string SurveyTypeId { get; set; }
        public SurveyType SurveyType { get; set; }
        public ICollection<SurveyQuestion> Questions { get; set; }
        public ICollection<UserSurvey> UserSurveys { get; set; }
    }
}
