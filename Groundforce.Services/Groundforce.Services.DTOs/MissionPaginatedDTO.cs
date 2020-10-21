using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
     public class MissionPaginatedDTO
    {
        public string Page { get; set; }
        public string Per_page { get; set; }
        public string TotalPages { get; set; }

        public IEnumerable<MissionDTO> Data { get; set; }

    }
}
