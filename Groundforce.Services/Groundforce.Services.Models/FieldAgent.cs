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
        public string PlaceOfBirth { get; set; }

        [Required]
        [MaxLength(150)]
        public string State { get; set; }

        [Required]
        [MaxLength(150)]
        public string LGA { get; set; }

        [Required]
        [MaxLength(200)]
        public string HomeAddress { get; set; }

        [Required]
        public string Longitude { get; set; }

        [Required]
        public string Latitude { get; set; }

        [Required]
        [MaxLength(25)]
        public string Religion { get; set; }

        [MaxLength(14)]
        public string AdditionalPhoneNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string AccountName { get; set; }

        [Required]
        [MaxLength(10)]
        public string AccountNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<Mission> Missions { get; set; }
        public ICollection<PointAllocated> PointsAllocated { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
