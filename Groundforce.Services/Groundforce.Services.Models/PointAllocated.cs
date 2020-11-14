using System;
using System.ComponentModel.DataAnnotations;


namespace Groundforce.Services.Models
{
    public class PointAllocated
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public string FieldAgentId { get; set; }
        public FieldAgent FieldAgent { get; set; }

        [Required]
        public string PointsId { get; set; }
        public Point Point { get; set; }
    }
}
