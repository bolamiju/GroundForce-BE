using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Groundforce.Services.Models
{
    public class FieldAgent
    {
        [Key]
        [Required]
        public string FieldAgentId { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }


        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public string Religion { get; set; }

        public string AdditionalPhoneNumber { get; set; }
        public BankAccount BankAccounts { get; set; }

        public ICollection<Mission> Missions { get; set; }
        public ICollection<PointAllocated> PointsAllocated { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
