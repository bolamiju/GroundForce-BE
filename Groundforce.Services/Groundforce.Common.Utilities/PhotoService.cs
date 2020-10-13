using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Groundforce.Common.Utilities
{
    public class PhotoService : IPhotoService
    {
        private Cloudinary _cloudinary;

        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;

        public PhotoService(IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;

            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public ImageUploadResult Upload(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.Name, stream),
                Transformation = new Transformation()           //  *

                    .Width(500).Height(500)
                    .Crop("fill")
                    .Gravity("face")
            };

            var uploadResult = _cloudinary.Upload(uploadParams);

            return uploadResult;
        }
    }
}