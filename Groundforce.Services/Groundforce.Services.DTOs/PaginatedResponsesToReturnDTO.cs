using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class PaginatedResponsesToReturnDTO
    {
        public PageMetaData PageMetaData { get; set; }

        public IEnumerable<ResponseToReturnDTO> Data { get; set; }

        public PaginatedResponsesToReturnDTO()
        {
            Data = new List<ResponseToReturnDTO>();
        }
    }
}
