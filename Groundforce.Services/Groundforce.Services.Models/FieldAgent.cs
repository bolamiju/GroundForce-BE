using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Groundforce.Services.Models
{
    public class FieldAgent
    {
        [Key]
        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [MaxLength(150)]
        public string State { get; set; }

        [Required]
        [MaxLength(150)]
        public string LGA { get; set; }

        [Required]
        [MaxLength(10)]
        public string ZipCode { get; set; }


        [MaxLength(200)]
        public string ResidentialAddress { get; set; }

        [Required]
        public string Longitude { get; set; }

        [Required]
        public string Latitude { get; set; }

        [MaxLength(14)]
        public string AdditionalPhoneNumber { get; set; }

        [MaxLength(100)]
        public string BankName { get; set; }

        [MaxLength(100)]
        public string AccountName { get; set; }

        [MaxLength(10)]
        public string AccountNumber { get; set; }

        public bool IsLocationVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<Mission> Missions { get; set; }
        public ICollection<PointAllocated> PointsAllocated { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
