using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Groundforce.Services.Models
{
    public class AssignedAddresses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Remarks { get; set; }
        public string Landmark { get; set; }
        public string BusStop { get; set; }
        public string BuildingColor { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool Accepted { get; set; }


        //Foriegn Keys
        [Required]
        [Display(Name = "Address Id")]
        public int AddressId { get; set; }
        public Address Address { get; set; }

        [Required]
        [Display(Name = "Building Type Id")]
        public int BuildingTypeId { get; set; }
        public BuildingType BuildingType { get; set; }

        [Required]
        [Display(Name = "Field Agent Id")]
        public int FieldAgentId { get; set; }
        [Display(Name = "Field Agent")]
        public FieldAgent FieldAgent { get; set; }

        [Display(Name = "Admin Id")]
        public int AdminId { get; set; }
        public Admin Admin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsAccepted { get; set; }
    }
}
