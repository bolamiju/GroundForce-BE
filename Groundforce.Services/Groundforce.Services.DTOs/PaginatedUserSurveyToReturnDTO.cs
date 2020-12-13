using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class PaginatedUserSurveyToReturnDTO
    {
        public PageMetaData PageMetaData { get; set; }
        public IEnumerable<UserSurveyToReturnDTO> UserSurveyToReturn { get; set; }
        public PaginatedUserSurveyToReturnDTO()
        {
            UserSurveyToReturn = new List<UserSurveyToReturnDTO>();
        }
    }
}
