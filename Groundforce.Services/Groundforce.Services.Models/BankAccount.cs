using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Groundforce.Services.Models
{
    public class BankAccount
    {
        [Key]
        [Required]
        public string AccountId { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public string AccountName { get; set; }

        [Required]
        [Display(Name = "AccountNumber")]
        public string AccountNumber { get; set; }

        public bool IsActive { get; set; } = false;

        [Required]
        [Display(Name = "Field Agent Id")]
        public string FieldAgentId { get; set; }

        public FieldAgent FieldAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
