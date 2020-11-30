using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class SurveyToReturnDTO
    {
        public string SurveyId { get; set; }
        public string Topic { get; set; }
        public ICollection<UserSurveyToReturnDTO> UserSurveys { get; set; }
        public ICollection<SurveyQuestionToReturnDTO> Questions { get; set; }
    }
}
