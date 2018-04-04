using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Services.Entities;
using Services.Interfaces;

namespace Services.Implementation
{
    public class CloudStorageService : ICloudStorageService
    {
        public IConfiguration Configuration { get; set; }

        private readonly string _imagesContainerName;
        private readonly string _thumbnailsContainerName;

        private readonly CloudStorageAccount _cloudStorageAccount;


        public CloudStorageService(IConfiguration configuration)
        {
            Configuration = configuration;

            var cloudConnectionString   = Configuration["BlobConnectionString"];

            _imagesContainerName = Configuration["ImageContainerName"];

            _thumbnailsContainerName = Configuration["ThumbnailsContainerName"];

            _cloudStorageAccount = CloudStorageAccount.Parse(cloudConnectionString);
        }


        public async Task CreateContainersIfNotExist()
        {
            var blobClient      = _cloudStorageAccount.CreateCloudBlobClient();
            var imagesContainer = blobClient.GetContainerReference(_imagesContainerName);

            await imagesContainer.CreateIfNotExistsAsync();

            var imagesContainerpermissions = await imagesContainer.GetPermissionsAsync();

            imagesContainerpermissions.PublicAccess = BlobContainerPublicAccessType.Container;

            await imagesContainer.SetPermissionsAsync(imagesContainerpermissions);


            var thumbnailsContainer = blobClient.GetContainerReference(_thumbnailsContainerName);

            await thumbnailsContainer.CreateIfNotExistsAsync();

            var thumbnailsContainerPermissions = await thumbnailsContainer.GetPermissionsAsync();

            thumbnailsContainerPermissions.PublicAccess = BlobContainerPublicAccessType.Container;

            await thumbnailsContainer.SetPermissionsAsync(thumbnailsContainerPermissions);


            var cloudTableClient = _cloudStorageAccount.CreateCloudTableClient();
            var cloudTable       = cloudTableClient.GetTableReference(_imagesContainerName);

            await cloudTable.CreateIfNotExistsAsync();
        }

        public async void UploadImage(IFormFile file)
        {
            var imageName = Guid.NewGuid();

            await UploadImageToBlob(file, imageName);
        }

        private async Task UploadImageToBlob(IFormFile file, Guid imageName)
        {
            var blobClient     = _cloudStorageAccount.CreateCloudBlobClient();
            var blobContainer  = blobClient.GetContainerReference(_imagesContainerName);

            var blockBlob  = blobContainer.GetBlockBlobReference(imageName.ToString());
            var stream     = file.OpenReadStream();

            blockBlob.UploadFromStreamAsync(stream).Wait();

            await UploadImageInformationToTable(file, imageName, blockBlob.Uri);
        }

        private async Task UploadImageInformationToTable(IFormFile file, Guid imageId, Uri blobUri)
        {
            var tableClient = _cloudStorageAccount.CreateCloudTableClient();
            var table       = tableClient.GetTableReference(_imagesContainerName);

            var fileName        = Path.GetFileName(file.FileName);
            var entity          = new Image(imageId) { Name = fileName, Uri = blobUri.AbsoluteUri };
            var insertOperation = TableOperation.Insert(entity);

            await table.ExecuteAsync(insertOperation);
        }

        public IQueryable<Image> RetrieveImages()
        {
            var tableClient = _cloudStorageAccount.CreateCloudTableClient();
            var table       = tableClient.GetTableReference(_imagesContainerName);

            var tableQuery       = new TableQuery<Image>();
            var tableQueryResult = table.ExecuteQuerySegmentedAsync(tableQuery, null).Result;

            return tableQueryResult.Results.AsQueryable();
        }

        public Image RetrieveImage(Guid id)
        {
            var tableClient = _cloudStorageAccount.CreateCloudTableClient();
            var table       = tableClient.GetTableReference(_imagesContainerName);

            var tableQuery       = new TableQuery<Image>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString()));
            var tableQueryResult = table.ExecuteQuerySegmentedAsync(tableQuery, null).Result;

            return tableQueryResult.Results.Single();
        }
    }
}