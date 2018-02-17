using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Services.Entities;
using Services.Interfaces;

namespace Services.Implementation
{
    public class CloudStorageService : ICloudStorageService
    {
        public IConfiguration Configuration { get; set; }

        private readonly string _accountName;
        private readonly string _key;
        private readonly string _containerName;
        private CloudStorageAccount _cloudStorageAccount;


        public CloudStorageService(IConfiguration configuration)
        {
            Configuration = configuration;

            _accountName   = Configuration["BlobAccountName"];
            _key           = Configuration["BlobStorageAPIKey"];
            _containerName = Configuration["ImageContainerName"];
        }

        public async void UploadImage(IFormFile file)
        {
            _cloudStorageAccount = new CloudStorageAccount(new StorageCredentials(_accountName, _key), true);

            var imageName = Guid.NewGuid();

            await UploadImageToBlob(file, imageName);
        }

        private async Task UploadImageToBlob(IFormFile file, Guid imageName)
        {
            var blobClient     = _cloudStorageAccount.CreateCloudBlobClient();
            var blobContainer  = blobClient.GetContainerReference(_containerName);
            var blockBlob      = blobContainer.GetBlockBlobReference(imageName.ToString());

            var stream = file.OpenReadStream();

            blockBlob.UploadFromStreamAsync(stream).Wait();

            await UploadImageInfomationToTable(file, imageName, blockBlob.Uri);
        }

        private async Task UploadImageInfomationToTable(IFormFile file, Guid imageId, Uri blobUri)
        {
            var tableClient = _cloudStorageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("images");

            await table.CreateIfNotExistsAsync();

            var entity = new Image(file.FileName, imageId) { Uri = blobUri.AbsoluteUri };

            var insertOperation = TableOperation.Insert(entity);

            await table.ExecuteAsync(insertOperation);
        }
    }
}