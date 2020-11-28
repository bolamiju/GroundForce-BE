using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ResponseToReturnDTO
    {
        public string ResponseId { get; set; }
        public string ApplicationUserId { get; set; }
        public string SurveyId { get; set; }
        public string SurveyQuestionId { get; set; }
        public string QuestionOptionId { get; set; }
    }
}
