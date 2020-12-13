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
        [Column(TypeName = "decimal(18,4)")]
        public decimal PointNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public ICollection<PointAllocated> PointsAllocated { get; set; }

    }
}
