using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Groundforce.Services.DTOs
{
    public class UserToRegisterDTO
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

        //additional Phonenumber
        [Required]
        public string AdditionalPhoneNumber { get; set; }

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
        [ValidateDOBFormat(ErrorMessage = "Invalid date of birth format. Must be dd/mm/yyyy")]
        [ValidateDOBRange(18, 120, ErrorMessage = "Age range allowed is 18 - 120")]
        public string DOB { get; set; }

        //Religion
        [Required]
        public string Religion { get; set; }

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

        //BankName
        [Required]
        [Display(Name = "Bank name")]
        public string BankName { get; set; }

        //AccountNumber
        [Required]
        [Display(Name = "Account number")]
        public string AccountNumber { get; set; }


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
            var age = DateTime.Now.Year - Convert.ToDateTime(value).Year;
            if (age > _maxAge || age < _minAge)
                return false;

            return true;
        }
    }

    class ValidateDOBFormatAttribute : ValidationAttribute
    {
        public ValidateDOBFormatAttribute(){}

        public override bool IsValid(object value)
        {
            var regex = @"^\d{1,2}/\d{1,2}/\d{4}$";
            string DOB = value.ToString();
            var response = Regex.Match(DOB, regex).Success;
            return response;
        }
    }
}
