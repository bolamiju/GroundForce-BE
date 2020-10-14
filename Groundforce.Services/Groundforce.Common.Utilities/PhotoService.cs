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
    public class PhotoService
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

        /// <summary>
        /// A method that uploads the photo file to cloudinaary
        /// </summary>
        /// <param name="file">the file to upload</param>
        /// <returns>The returned response</returns>
        public ImageUploadResult Upload(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    // change file description
                    File = new FileDescription(file.Name, stream),
                    Transformation = new Transformation()

                        .Width(500).Height(500)
                        .Crop("fill")
                        .Gravity("face")
                };

                //upload the file
                uploadResult = _cloudinary.Upload(uploadParams);
            }

            return uploadResult;
        }
    }
}