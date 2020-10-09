using System;
using System.ComponentModel.DataAnnotations;

namespace Groundforce.Services.API.DTOs
{
    /// <summary>
    /// defines data transfer object for registration of a user
    /// </summary>
    public class RegistrationDTO
    {

        [Required]
        [MaxLength(50, ErrorMessage = "Last name must not be more than 50 characters")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "First name must not be more than 50 characters")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Last name must not be more than 50 characters")]
        [Display(Name = "Date of birth")]
        [ValidateDOBRange(18, 120, ErrorMessage = "Age range allowed is 18 - 120")]
        public string DOB { get; set; }
        [Required]
        public string Religion { get; set; }

        public string AdditionalPhoneNumber { get; set; }

        [Required]
        [Display(Name = "Home address")]
        public string HomeAddress { get; set; }

        [Required]
        [Display(Name = "Bank name")]
        public string BankName { get; set; }

        [Required]
        [Display(Name = "Account number")]
        public string AccountNumber { get; set; }
        [Required]
        [Display(Name = "Place of birth")]
        public string PlaceOfBirth { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        [Display(Name = "Local Government Area")]
        public string LGA { get; set; }

        [Required]
        [MaxLength(4)]
        public string PIN { get; set; }
        public string Longitude { get; set; }
        [Required]
        public string Latitude { get; set; }
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
}
