using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly IConfiguration _appConfig;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotoRepository(IOptions<CloudinarySettings> cloudinaryConfig, IConfiguration configuration )
        {
            _appConfig = configuration;
            _cloudinaryConfig = cloudinaryConfig;

            // setup the account object
            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(account);
        }

        public ImageUploadResult UploadPix(IFormFile Picture)
        {
            // validate the pix size and extension type using settings from appsettings
            var pictureSizeCheck = false;
            var listOfExtensions = _appConfig.GetSection("PhotoSettings:Extensions").Get<List<string>>();
            for (int i = 0; i < listOfExtensions.Count; i++)
            {
                if (Picture.FileName.EndsWith(listOfExtensions[i]))
                {
                    pictureSizeCheck = true;
                    break;
                }
            }

            var pixSize = Convert.ToInt64(_appConfig.GetSection("PhotoSettings:Size").Get<string>());

            if (Picture == null || Picture.Length > pixSize)
                throw new Exception("File size should not exceed 2mb");

            if (!pictureSizeCheck)
                throw new Exception("File format is not supported. Please upload a picture");


            /// object to return as result
            var uploadResult = new ImageUploadResult();

            // fetch image as stream of data
            using (var stream = Picture.OpenReadStream())
            {
                /// set image upload params
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(Picture.Name, stream),
                    Transformation = new Transformation()
                                        .Width(500)
                                        .Height(500)
                                        .Crop("fill")
                                        .Gravity("face")
                };
                // upload to cloudinary
                uploadResult = _cloudinary.Upload(uploadParams);
            }

            return uploadResult;           

        }
    }
}
