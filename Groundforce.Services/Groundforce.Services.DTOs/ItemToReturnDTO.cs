using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ItemToReturnDTO
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string AddedBy { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
