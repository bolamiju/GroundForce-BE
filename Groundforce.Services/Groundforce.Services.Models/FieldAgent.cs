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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FieldAgentId { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public string Longitude { get; set; }
        [Required]
        public string Latitude { get; set; }
        public BankAccount BankAccounts { get; set; }
        public ICollection<AssignedAddresses> AssignedAddresses { get; set; }
        public ICollection<PointAllocated> PointsAllocated { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
