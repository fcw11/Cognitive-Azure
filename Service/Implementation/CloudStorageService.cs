using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Services.Interfaces;

namespace Services.Implementation
{
    public class CloudStorageService : ICloudStorageService
    {
        public IConfiguration Configuration { get; set; }

        private ICloudTableService CloudTableService { get; }

        private readonly string _imagesContainerName;
        private readonly string _thumbnailsContainerName;
        private readonly string _facesContainerName;

        private readonly CloudStorageAccount _cloudStorageAccount;
        
        public CloudStorageService(IConfiguration configuration, ICloudTableService cloudTableService)
        {
            Configuration = configuration;

            CloudTableService = cloudTableService;

            var cloudConnectionString   = Configuration["BlobConnectionString"];

            _imagesContainerName = Configuration["ImageContainerName"];

            _thumbnailsContainerName = Configuration["ThumbnailsContainerName"];

            _facesContainerName = Configuration["FacesContainerName"];

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


            var facesContainer = blobClient.GetContainerReference(_facesContainerName);

            await facesContainer.CreateIfNotExistsAsync();

            var facesContainerPermissions = await facesContainer.GetPermissionsAsync();

            facesContainerPermissions.PublicAccess = BlobContainerPublicAccessType.Container;

            await facesContainer.SetPermissionsAsync(facesContainerPermissions);
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

            await CloudTableService.UploadImageInformationToTable(file, imageName, blockBlob.Uri);
        }
    }
}