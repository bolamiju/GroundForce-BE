using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.Models
{
    public class MissionVerified
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string MissionId { get; set; }
        public Mission Mission { get; set; }

        [Required]
        public string BuildingTypeId { get; set; }
        public BuildingType BuildingType { get; set; }

        [Required]
        [MaxLength(150)]
        public string Landmark { get; set; }
        [Required]
        [MaxLength(150)]
        public string BusStop { get; set; }

        [Required]
        public string BuildingColor { get; set; }

        [Required]
        public bool AddressExists { get; set; } = false;

        [Required]
        [MaxLength(35)]
        public string TypeOfStructure { get; set; }
        
        [Required]
        public string Longitude { get; set; }
        
        [Required]
        public string Latitude { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
