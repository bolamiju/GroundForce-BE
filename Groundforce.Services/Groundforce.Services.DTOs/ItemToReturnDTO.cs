using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ItemToReturnDTO
    {
        public string ItemId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AddedBy { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
