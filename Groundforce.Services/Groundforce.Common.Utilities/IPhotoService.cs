using System;
using System.Collections.Generic;
using System.Text;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Groundforce.Common.Utilities
{
    public interface IPhotoService
    {
        public ImageUploadResult Upload(IFormFile file);
    }
}