using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class UserWithoutDetailsDTO
    {
        //lastname
        [Required]
        [MaxLength(50, ErrorMessage = "Last name must not be more than 50 characters")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        //firstname
        [Required]
        [MaxLength(50, ErrorMessage = "First name must not be more than 50 characters")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }

        //gender
        [Required]
        public string Gender { get; set; }

        //emailAddress
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //DOB which is Date Of Birth
        [Required]
        [Display(Name = "Date of birth")]
        [ValidateDOBRange(18, 120, ErrorMessage = "Age range allowed is 18 - 120")]
        public string DOB { get; set; }

        //State
        [Required]
        public string State { get; set; }

        //PlaceOfBirth
        [Required]
        [Display(Name = "Place of Birth")]
        public string PlaceOfBirth { get; set; }

        //LGA
        [Required]
        [Display(Name = "Local Government Area")]
        public string LGA { get; set; }

        //HomeAdress
        [Required]
        [Display(Name = "Home address")]
        public string HomeAddress { get; set; }


        //Pin: Used as Password
        [Required]
        [MaxLength(4)]
        [MinLength(4)]
        [DataType(DataType.Password)]
        public string PIN { get; set; }
    }
}
