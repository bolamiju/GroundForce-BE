using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class PaginatedSurveysToReturnDTO
    {
        public PageMetaData PageMetaData { get; set; }

        public IEnumerable<SurveyToReturnDTO> Data { get; set; }

        public PaginatedSurveysToReturnDTO()
        {
            Data = new List<SurveyToReturnDTO>();
        }
    }
}
