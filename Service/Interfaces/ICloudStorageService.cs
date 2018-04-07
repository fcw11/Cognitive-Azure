using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Services.Entities;

namespace Services.Interfaces
{
    public interface ICloudStorageService
    {
        Task CreateContainersIfNotExist();

        void UploadImage(IFormFile file);
        IQueryable<Image> RetrieveImages();
        Image RetrieveImage(Guid id);
    }
}
