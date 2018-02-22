using System.Linq;
using Microsoft.AspNetCore.Http;
using Services.Entities;

namespace Services.Interfaces
{
    public interface ICloudStorageService
    {
        void UploadImage(IFormFile file);
        IQueryable<Image> RetrieveImages();
    }
}
