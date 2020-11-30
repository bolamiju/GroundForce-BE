using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class PaginatedQuestionOptionToReturnDTO
    {
        public PageMetaData PageMetaData { get; set; }
        public IEnumerable<QuestionOptionDTO> QuestionOptionToReturn { get; set; }
        public PaginatedQuestionOptionToReturnDTO()
        {
            QuestionOptionToReturn = new List<QuestionOptionDTO>();
        }
    }
}
