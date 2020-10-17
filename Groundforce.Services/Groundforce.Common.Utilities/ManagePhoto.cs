using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Common.Utilities
{
    public class ManagePhoto
    {
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public ManagePhoto(IOptions<CloudinarySettings> cloudinaryConfig)
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
        /// uploads user photo
        /// </summary>
        /// <param name="avarta"></param>
        /// <returns>url of the picture</returns>
        public ImageUploadResult UploadAvatar(IFormFile avarta)
        {
            var uploadResult = new ImageUploadResult();

            using (var stream = avarta.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(avarta.Name, stream),
                    Transformation = new Transformation()
                                        .Width(500)
                                        .Height(500)
                                        .Crop("fill")
                                        .Gravity("face")
                };
                uploadResult = _cloudinary.Upload(uploadParams);
            }
            
            return uploadResult;
        }
    }
}
