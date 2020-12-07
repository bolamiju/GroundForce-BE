using System;
using System.ComponentModel.DataAnnotations;

namespace Groundforce.Services.Models
{
    public class Mission
    {
        // AddedBy UpdatedBy
        [Key]
        [Required]
        public string MissionId { get; set; }

        //Foriegn Keys
        [Required]
        public string VerificationItemId { get; set; }
        public VerificationItem VerificationItem { get; set; }

        [Required]
        public string FieldAgentId { get; set; }
        public FieldAgent FieldAgent { get; set; }

        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }

        public string VerificationStatus { get; set; } = "pending"; // pending, accepted, declined, verified

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public MissionVerified MissionVerified { get; set; }
    }
}
