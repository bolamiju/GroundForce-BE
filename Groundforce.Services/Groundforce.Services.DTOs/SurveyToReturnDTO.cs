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
        public List<string> Questions { get; set; }

        public SurveyToReturnDTO()
        {
            Questions = new List<string>();
        }
    }
}
