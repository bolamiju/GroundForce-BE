using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.Models
{
    public class Response
    {
        [Key]
        [Required]
        public string ResponseId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public string SurveyId { get; set; }
        public Survey Survey { get; set; }

        [Required]
        public string SurveyQuestionId { get; set; }
        public SurveyQuestion SurveyQuestion { get; set; }

        [Required]
        public string QuestionOptionId { get; set; }
        public QuestionOption QuestionOption { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public UserSurvey UserSurvey { get; set; }
    }
}
