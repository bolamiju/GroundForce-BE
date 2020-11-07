using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class UserLocationDTO
    {
        //Longitude
        [Required]
        public string Longitude { get; set; }

        //LAtitude
        [Required]
        public string Latitude { get; set; }

    }
}
