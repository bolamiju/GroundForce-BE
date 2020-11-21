using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class MissionToVerifyDTO
    {
        [Required]
        public string MissionId { get; set; }
        public Mission Mission { get; set; }

        [Required]
        public string BuildingTypeId { get; set; }
        public BuildingType BuildingType { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage ="Landmark must not be more than 150 characters long")]
        public string Landmark { get; set; }
        [Required]
        [MaxLength(150, ErrorMessage = "Bus-stop must not be more than 150 characters long")]
        public string BusStop { get; set; }

        [Required]
        public string BuildingColor { get; set; }

        [Required]
        public bool AddressExists { get; set; } = false;

        [Required]
        [MaxLength(35, ErrorMessage = "Structure-type must not be more than 35 characters long")]
        public string TypeOfStructure { get; set; }

        [Required]
        public string Longitude { get; set; }

        [Required]
        public string Latitude { get; set; }

        [Required]
        public string Remarks { get; set; }
    }
}
