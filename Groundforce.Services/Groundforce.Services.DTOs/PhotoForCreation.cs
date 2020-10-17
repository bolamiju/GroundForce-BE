using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Services.DTOs
{
    public class PhotoForCreation
    {
        public IFormFile PhotoFile { get; set; }
    }
}