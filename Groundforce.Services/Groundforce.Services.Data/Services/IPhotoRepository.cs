using CloudinaryDotNet.Actions;
using Groundforce.Services.DTOs;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IPhotoRepository
    {
        ImageUploadResult UploadPix(IFormFile Picture);
    }
}
