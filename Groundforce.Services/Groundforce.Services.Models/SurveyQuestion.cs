using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.Models
{
    public class SurveyQuestion
    {
        public string SurveyQuestionId { get; set; } 
        public string Question { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Survey Id")]
        public string SurveyId { get; set; }
        public Survey Survey { get; set; }
        public ICollection<QuestionOption> QuestionOptions { get; set; }
    }
}
