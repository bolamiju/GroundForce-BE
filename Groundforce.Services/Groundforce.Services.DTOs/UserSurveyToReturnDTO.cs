using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class UserSurveyToReturnDTO
    {
        public string Topic { get; set; }
        public string ApplicationUserId { get; set; }
        public string SurveyId { get; set; }
        public string Status { get; set; }
    }
}
