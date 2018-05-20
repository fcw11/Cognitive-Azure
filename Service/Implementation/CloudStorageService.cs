using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Services.Interfaces;

namespace Services.Implementation
{
    public class CloudStorageService : ICloudStorageService
    {
        public IConfiguration Configuration { get; set; }

        private CloudBlobClient CloudBlobClient { get; }

        private ICloudTableService CloudTableService { get; }
        
        public CloudStorageService(IConfiguration configuration, ICloudTableService cloudTableService)
        {
            Configuration = configuration;

            CloudTableService = cloudTableService;

            var cloudConnectionString = Configuration["BlobConnectionString"];

            var cloudStorageAccount = CloudStorageAccount.Parse(cloudConnectionString);

            CloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }
        
        public async Task CreateContainersIfNotExist()
        {          
            var list = new List<string>
            {
                "ThumbnailsContainerName",
                "FacesContainerName",
                "ImagesContainerName"
            };

            foreach (var configurationName in list)
            {
                var containerName = Configuration[configurationName];

                var container = CloudBlobClient.GetContainerReference(containerName);

                await container.CreateIfNotExistsAsync();

                var imagesContainerpermissions = await container.GetPermissionsAsync();

                imagesContainerpermissions.PublicAccess = BlobContainerPublicAccessType.Container;

                await container.SetPermissionsAsync(imagesContainerpermissions);
            }
        }

        public CloudBlob Upload(Stream stream, Guid id, string containerName)
        {
            var blobContainer = CloudBlobClient.GetContainerReference(containerName);

            var blockBlob = blobContainer.GetBlockBlobReference(id.ToString());

            blockBlob.UploadFromStreamAsync(stream).Wait();
           
            return blockBlob;
        }
    }
}