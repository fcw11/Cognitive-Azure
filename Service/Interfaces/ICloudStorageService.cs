using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Services.Interfaces
{
    public interface ICloudStorageService
    {
        Task CreateContainersIfNotExist();

        void UploadImage(IFormFile file);
    }
}