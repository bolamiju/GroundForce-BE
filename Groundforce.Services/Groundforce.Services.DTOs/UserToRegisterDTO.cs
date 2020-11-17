using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Groundforce.Services.DTOs
{
    public class UserToRegisterDTO
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Last name must not be more than 50 characters")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "First name must not be more than 50 characters")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [MaxLength(1, ErrorMessage ="Max length 1")]
        [RegularExpression(@"\w{1}", ErrorMessage = "Gender should be a single character eg: m or f")]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Date of birth")]
        [ValidateDOBRange(18, 120, ErrorMessage = "Ensure date format is dd/mm/yyyy and age is between 18 - 120")]
        public string DOB { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        [MaxLength(14, ErrorMessage = "Phone number must not be 14 characters")]
        [RegularExpression(@"^\+\d{3}\d{9,10}$", ErrorMessage = "Must have country-code and must be 13, 14 chars long e.g. +2348050000000")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(4, ErrorMessage = "Pin must be exactly 4 digits", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Pin must all be digits")]
        public string PIN { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage = "State must not be 150 characters")]
        public string State { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage = "State must not be 150 characters")]
        public string LGA { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "State must not be 10 characters")]
        public string ZipCode { get; set; }

        [Required]
        public string Longitude { get; set; }

        [Required]
        public string Latitude { get; set; }

        public List<string> Roles { get; set; }


        public UserToRegisterDTO()
        {
            Roles = new List<string>();
        }


    }

    class ValidateDOBRangeAttribute : ValidationAttribute
    {
        private int _minAge; private int _maxAge;
        public ValidateDOBRangeAttribute(int minAge = 18, int maxAge = 120)
        {
            _maxAge = maxAge;
            _minAge = minAge;
        }

        public override bool IsValid(object value)
        {
            var regex = @"^\d{1,2}/\d{1,2}/\d{4}$";
            string DOB = value.ToString();
            if (!Regex.Match(DOB, regex).Success)
                return false;

            var age = DateTime.Now.Year - Convert.ToDateTime(value).Year;
            if (age > _maxAge || age < _minAge)
                return false;

            return true;
        }
    }

}
