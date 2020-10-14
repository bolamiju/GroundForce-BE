using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ForgotUserPwdDTO
    {
        [Required]
        public string phoneNumber { get; set; }
        [Required]
        [MaxLength(4, ErrorMessage = "Pin must not be longer than 4")]
        [DataType(DataType.Password)]
        public string newPin { get; set; }      
    }
}
