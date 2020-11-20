using Groundforce.Services.Models;
using System.Collections.Generic;

namespace Groundforce.Services.DTOs
{
    public class PaginatedItemsToReturnDTO
    {
        public PageMetaData PageMetaData { get; set; }

        public IEnumerable<ItemToReturnDTO> Data { get; set; }

        public PaginatedItemsToReturnDTO()
        {
            Data = new List<ItemToReturnDTO>();
        }
    }
}
