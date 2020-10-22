using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class PaginationDTO
    {

        public int PerPage { get; set; }
        public int Page { get; set; }

        public int TotalPage { get; set; }
    }
}
