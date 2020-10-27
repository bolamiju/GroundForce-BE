using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Groundforce.Services.Models
{
    public class Point
    {
        [Key]
        public string PointId { get; set; }

        [Required]
        [Display(Name = "Amount Attached")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal AmountAttached { get; set; }

        [Required]
        [Display(Name = "Admin Id")]
        public string AdminId { get; set; }
        public Admin Admin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public ICollection<PointAllocated> PointsAllocated { get; set; }

    }
}
