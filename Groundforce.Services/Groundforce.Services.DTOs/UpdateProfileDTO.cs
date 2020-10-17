using System.ComponentModel.DataAnnotations;

namespace Groundforce.Services.DTOs
{
    public class UpdateProfileDTO
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Last name must not be more than 50 characters")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        //firstname
        [Required]
        [MaxLength(50, ErrorMessage = "First name must not be more than 50 characters")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        //additional Phonenumber
        [Required]
        public string AdditionalPhoneNumber { get; set; }
        //DOB which is Date Of Birth
        [Required]
        [Display(Name = "Date of birth")]
        [ValidateDOBRange(18, 120, ErrorMessage = "Age range allowed is 18 - 120")]
        public string DOB { get; set; }
        public string ResidentialAddress { get; set; }
        //Religion
        [Required]
        public string Religion { get; set; }
        //gender
        [Required]
        public string Gender { get; set; }
        //emailAddress
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        //BankName
        [Required]
        [Display(Name = "Bank name")]
        public string BankName { get; set; }
        //AccountNumber
        [Required]
        [Display(Name = "Account number")]
        public string AccountNumber { get; set; }
    }
}
