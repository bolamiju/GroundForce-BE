using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.Models
{
    public class ApplicationUser : IdentityUser
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
        public string Gender { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Last name must not be more than 50 characters")]
        [Display(Name = "Date of birth")]
        [ValidateDOBRange(18, 120, ErrorMessage = "Age range allowed is 18 - 120")]
        public string DOB { get; set; }

        [Required]
        [Display(Name = "Place of birth")]
        public string PlaceOfBirth { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [Display(Name = "Local Government Area")]
        public string LGA { get; set; }

        [Required]
        [Display(Name = "Home address")]
        public string HomeAddress { get; set; }

        public bool IsAccountActive { get; set; } = false;
        public string AvatarUrl { get; set; }
        public string PublicId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public FieldAgent FieldAgent { get; set; }
        public Admin Admin { get; set; }
        public Client Client { get; set; }
        public ICollection<VerificationItem> VerificationItems { get; set; }

    }

    class ValidateDOBRangeAttribute : ValidationAttribute
    {
        private int _minAge; private int _maxAge;
        public ValidateDOBRangeAttribute(int minAge=18, int maxAge=120)
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
