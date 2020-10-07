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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string Reference { get; set; }
        [Required]
        [Display(Name = "Paid Amount")]
        public int PaidAmount { get; set; }
        [Required]
        [Display(Name = "Paid At")]
        public DateTime PaidAt { get; set; }
        [Required]
        [Display(Name = "Actual Amount")]
        public int ActualAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Field Agent Id")]
        public int FieldAgentId { get; set; }
        public FieldAgent FieldAgent { get; set; }

        [Required]
        [Display(Name = "Admin Id")]
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}
