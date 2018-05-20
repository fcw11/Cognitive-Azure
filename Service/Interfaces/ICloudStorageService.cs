using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Services.Interfaces
{
    public interface ICloudStorageService
    {
        Task CreateContainersIfNotExist();

        CloudBlob Upload(Stream entity, Guid id, string containerName);
    }
}