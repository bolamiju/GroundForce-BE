using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class FetchPaginatedMissionDTO
    {
        public PaginationDTO PaginationItems { get; set; }
        public List<AssignedAddresses> Data { get; set; }
    }
}
