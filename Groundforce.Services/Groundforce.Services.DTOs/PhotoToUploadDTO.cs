using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class PhotoToUploadDTO
    {
        public IFormFile Photo { get; set; }
    }
}
