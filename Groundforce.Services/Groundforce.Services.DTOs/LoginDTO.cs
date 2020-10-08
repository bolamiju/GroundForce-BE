using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Groundforce.Services.DTOs
{
    public class LoginDTO
    {

        [Required]
        public string Email { get; set; }
        [Required]
        public int Pin { get; set; }

    }
}
