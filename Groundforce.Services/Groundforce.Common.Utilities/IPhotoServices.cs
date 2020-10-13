using Microsoft.AspNetCore.Http;

namespace Groundforce.Common.Utilities
{
    public interface IPhotoServices
    {
        string UploadAvatar(IFormFile avarta);
    }
}