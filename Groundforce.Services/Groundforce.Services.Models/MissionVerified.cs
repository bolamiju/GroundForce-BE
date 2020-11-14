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

        public string MissionId { get; set; }
        public Mission Mission { get; set; }

        public string BuildingTypeId { get; set; }
        public BuildingType BuildingType { get; set; }

        [MaxLength(150)]
        public string Landmark { get; set; }

        [MaxLength(150)]
        public string BusStop { get; set; }

        [MaxLength(20)]
        public string BuildingColor { get; set; }

        public bool AddressExists { get; set; } = false;

        [MaxLength(35)]
        public string TypeOfStructure { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public string Remarks { get; set; }


    }
}
