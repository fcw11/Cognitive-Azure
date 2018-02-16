using Microsoft.AspNetCore.Http;

namespace Services.Interfaces
{
    public interface ICloudStorageService
    {
        void UploadImage(IFormFile file);
    }
}
