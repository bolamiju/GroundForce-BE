using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace Groundforce.Services.Models
{
    public class PointAllocated
    {
       [ Key]
        [Required]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Field Agent Id")]
        public string FieldAgentId { get; set; }
        public FieldAgent FieldAgent { get; set; }

        [Required]
        [Display(Name = "Admin Id")]
        public string AdminId { get; set; }
        public Admin Admin { get; set; }

        [Required]
        [Display(Name = "Point Id")]
        public string PointsId { get; set; }
        public Point Point { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
