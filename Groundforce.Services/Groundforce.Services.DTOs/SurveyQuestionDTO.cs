using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class SurveyQuestionDTO
    {
        public string Question { get; set; }
        public string SurveyId { get; set; }
        public List<string> Options { get; set; }
    }
}
