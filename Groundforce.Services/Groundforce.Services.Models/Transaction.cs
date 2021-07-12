using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Groundforce.Services.Models;

namespace Groundforce.Services.Models
{
    public class Transaction
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public string FieldAgentId { get; set; }
        public FieldAgent FieldAgent { get; set; }

        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }

        [Required]
        public string Reference { get; set; }

        [Required]
        public int PaidAmount { get; set; }

        [Required]
        public DateTime PaidAt { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal ActualAmount { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
