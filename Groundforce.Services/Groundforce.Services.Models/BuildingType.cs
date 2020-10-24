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
        public string TypeName { get; set; }

        [Required]
        [Display(Name = "Admin Id")]
        public string AdminId { get; set; }
        public Admin Admin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public ICollection<Mission> Missions { get; set; }
    }
}
