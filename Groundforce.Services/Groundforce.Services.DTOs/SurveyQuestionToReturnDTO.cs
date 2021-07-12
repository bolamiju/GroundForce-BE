using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class SurveyQuestionToReturnDTO
    {
        public string SurveyQuestionId { get; set; }
        public string SurveyId { get; set; }
        public string Question { get; set; }
        public ICollection<QuestionOptionDTO> Options { get; set; }
    }
}
