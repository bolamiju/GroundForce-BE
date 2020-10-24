using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Groundforce.Services.Models
{
    public class Admin
    {
        [Key]
        [Required]
        public string AdminId { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<BuildingType> BuildingTypes { get; set; }
        public ICollection<Mission> Missions { get; set; }
        public ICollection<Point> Points { get; set; }
        public ICollection<PointAllocated> PointsAllocated { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

    }
}
