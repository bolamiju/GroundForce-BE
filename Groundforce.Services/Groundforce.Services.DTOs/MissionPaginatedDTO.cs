using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
     public class MissionPaginatedDTO
    {
   

        public PaginationDTO Pagination { get; set; }

        public IEnumerable<MissionDTO> Data { get; set; }

    }
}
