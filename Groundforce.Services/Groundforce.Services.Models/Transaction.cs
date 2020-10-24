﻿using System;
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
        [Column(TypeName = "decimal(18,4)")]
        public decimal ActualAmount { get; set; }

        [Required]
        [Display(Name = "Field Agent Id")]
        public string FieldAgentId { get; set; }
        public FieldAgent FieldAgent { get; set; }

        [Required]
        [Display(Name = "Admin Id")]
        public string AdminId { get; set; }
        public Admin Admin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
