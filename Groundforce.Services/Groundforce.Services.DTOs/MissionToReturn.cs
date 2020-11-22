
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class MissionToReturn
    {
        public string MissionId { get; set; }
        public string ItemId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AddedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
