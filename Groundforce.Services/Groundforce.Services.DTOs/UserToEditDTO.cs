using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Groundforce.Services.DTOs
{
    public class UserToEditDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string AdditionalPhoneNumber { get; set; }
        public string Religion { get; set; }
    }

}
