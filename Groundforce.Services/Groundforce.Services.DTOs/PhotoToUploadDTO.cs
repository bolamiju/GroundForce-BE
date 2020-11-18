using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class PhotoToUploadDTO
    {
        [Required]
        public IFormFile Photo { get; set; }
    }
}
