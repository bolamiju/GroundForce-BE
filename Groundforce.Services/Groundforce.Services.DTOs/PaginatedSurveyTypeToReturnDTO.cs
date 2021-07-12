using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class PaginatedSurveyTypeToReturnDTO
    {
        public PageMetaData PageMetaData { get; set; }
        public IEnumerable<SurveyTypeDTO> SurveyTypeToReturn { get; set; }
        public PaginatedSurveyTypeToReturnDTO()
        {
            SurveyTypeToReturn = new List<SurveyTypeDTO>();
        }
    }
}
