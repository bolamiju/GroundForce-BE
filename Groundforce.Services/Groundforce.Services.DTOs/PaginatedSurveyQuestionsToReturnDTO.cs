using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class PaginatedSurveyQuestionsToReturnDTO
    {
        public PageMetaData PageMetaData { get; set; }

        public IEnumerable<SurveyQuestionToReturnDTO> Data { get; set; }

        public PaginatedSurveyQuestionsToReturnDTO()
        {
            Data = new List<SurveyQuestionToReturnDTO>();
        }
    }
}
