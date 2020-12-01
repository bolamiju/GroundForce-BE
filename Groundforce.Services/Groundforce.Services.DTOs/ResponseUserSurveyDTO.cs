using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ResponseUserSurveyDTO
    {
        public string AgentId { get; set; }
        public string SurveyId { get; set; }
        public List<ResponseDTO> Questions { get; set; }
    }
}
