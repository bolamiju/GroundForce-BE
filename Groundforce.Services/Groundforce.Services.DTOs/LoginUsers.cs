using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Groundforce.Services.API.DTOs
{
    public class LoginUsers
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Pin { get; set; }
    }
}
