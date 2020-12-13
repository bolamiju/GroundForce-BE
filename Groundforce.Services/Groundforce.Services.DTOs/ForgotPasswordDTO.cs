using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class ForgotPasswordDTO
    {
        [Required]
        public string EmailAddress { get; set; } 
    }
}
