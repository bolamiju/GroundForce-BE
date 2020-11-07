using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class UserPinDTO
    {
        //Pin: Used as Password
        [Required]
        [MaxLength(4)]
        [MinLength(4)]
        [DataType(DataType.Password)]
        public string PIN { get; set; }
    }
}
