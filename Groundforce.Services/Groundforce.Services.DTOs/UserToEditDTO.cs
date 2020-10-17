using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Groundforce.Services.DTOs
{
    public class UserToEditDTO
    {
        [Required]
        public string Gender { get; set; }

        //emailAddress
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //additional Phonenumber
        [Required]
        public string AdditionalPhoneNumber { get; set; }

        //Religion
        [Required]
        public string Religion { get; set; }
    }

}
