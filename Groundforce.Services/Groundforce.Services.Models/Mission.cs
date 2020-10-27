using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Groundforce.Services.Models
{
    public class Mission
    {
        [Key]
        [Required]
        public string MissionId { get; set; }
        public string Landmark { get; set; }
        public string BusStop { get; set; }
        public string BuildingColor { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsAccepted { get; set; } = false;
        public bool AddressExists { get; set; } = false;
        public string TypeOfStructure { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Remarks { get; set; }


        //Foriegn Keys
        [Required]
        [Display(Name = "Address Id")]
        public string VerificationItemId { get; set; }
        public VerificationItem VerificationItem { get; set; }

        [Display(Name = "Building Type Id")]
        public string BuildingTypeId { get; set; }
        public BuildingType BuildingType { get; set; }

        [Required]
        [Display(Name = "Field Agent Id")]
        public string FieldAgentId { get; set; }
        [Display(Name = "Field Agent")]
        public FieldAgent FieldAgent { get; set; }

        [Display(Name = "Admin Id")]
        public string AdminId { get; set; }
        public Admin Admin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
