using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Groundforce.Services.Models
{
    public class BuildingType
    {
        [Required]
        [Key]
        public string TypeId { get; set; }

        [Required]
        [MaxLength(35)]
        public string TypeName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public ICollection<MissionVerified> MissionsVerified { get; set; }
    }
}
