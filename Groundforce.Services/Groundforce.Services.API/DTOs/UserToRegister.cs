using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Groundforce.Services.API.DTOs
{
    public class UserToRegister
    {
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string Religion { get; set; }

        public string AdditionalPhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string PlaceOfBirth { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string LGA { get; set; }

        [Required]
        public string BankName { get; set; }

        [Required]
        public int AccountNumber { get; set; }

        [Required]
        [StringLength(4)]
        public string PIN { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}