using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Services.Interfaces;

namespace Services.Implementation
{
    public class CloudStorageService : ICloudStorageService
    {
        public IConfiguration Configuration { get; set; }

        private readonly string _accountName;
        private readonly string _key;
        private readonly string _containerName;

        public CloudStorageService(IConfiguration configuration)
        {
            Configuration = configuration;

            _accountName   = Configuration["BlobAccountName"];
            _key           = Configuration["BlobStorageAPIKey"];
            _containerName = Configuration["ImageContainerName"];
        }

        public void UploadImage(IFormFile file)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials(_accountName, _key), true);
            var blobClient     = storageAccount.CreateCloudBlobClient();
            var container      = blobClient.GetContainerReference(_containerName);
            var blockBlob      = container.GetBlockBlobReference(Guid.NewGuid().ToString());

            using (var stream = file.OpenReadStream())
            {
                blockBlob.UploadFromStreamAsync(stream).Wait();
            }
        }
    }
}