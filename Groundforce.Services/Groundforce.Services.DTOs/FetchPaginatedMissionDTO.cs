using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class FetchPaginatedMissionDTO : PaginationDTO
    {
        public List<AssignedAddresses> AssignedAddresses { get; set; }
    }
}
