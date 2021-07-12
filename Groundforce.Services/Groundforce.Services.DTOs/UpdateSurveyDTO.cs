using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class UpdateSurveyDTO
    {
        public string Topic { get; set; }
        public string SurveyId { get; set; }
        public string SurveyTypeId { get; set; }
    }
}
