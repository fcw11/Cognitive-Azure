using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.Entities;
using Services.Interfaces;

namespace Services.Implementation
{
    public class ImageService : IImageService
    {
        public IConfiguration Configuration { get; set; }

        public ICloudTableService CloudTableService { get; set; }

        public ICloudStorageService CloudStorageService { get; set; }

        public ImageService(IConfiguration configuration, ICloudTableService cloudTableService, ICloudStorageService cloudStorageService)
        {
            Configuration = configuration;

            CloudTableService = cloudTableService;

            CloudStorageService = cloudStorageService;
        }

        public async Task UploadImage(IFormFile file)
        {
            var imagesContainerName = Configuration["ImagesContainerName"];

            var imageName = Guid.NewGuid();

            using (var stream = file.OpenReadStream())
            {
                var blob = CloudStorageService.Upload(stream, imageName, imagesContainerName);

                await UploadImageInformationToTable(file, imageName, blob.Uri);
            }
        }

        public async Task UploadImageInformationToTable(IFormFile file, Guid imageId, Uri blobUri)
        {
            var tableName = Configuration["ImagesTable"];

            var fileName = Path.GetFileName(file.FileName);

            var entity = new Image(imageId) { Name = fileName, Uri = blobUri.AbsoluteUri };

            await CloudTableService.Insert(entity, tableName);
        }

        public async Task<IQueryable<Image>> RetrieveImages()
        {
            var imagesContainerName = Configuration["ImagesTable"];

            return await CloudTableService.Retrieve<Image>(imagesContainerName);
        }

        public async Task<Image> RetrieveImage(Guid id)
        {
            var tableName = Configuration["ImagesTable"];

            var image = await CloudTableService.RetrieveSingle<Image>(id, tableName);

            image.Comments = await RetrieveComments(id);

            return image;
        }

        public async Task<IQueryable<ImageComment>> RetrieveComments(Guid id)
        {
            var commentTable = Configuration["CommentsTable"];

            return await CloudTableService.Retrieve<ImageComment>(id, commentTable);
        }

        public async Task AddComment(Guid imageId, string comment, double score, string phrases)
        {
            var commentTable = Configuration["CommentsTable"];

            var imageComment = new ImageComment
            {
                PartitionKey = imageId.ToString(),
                Comment = comment,
                ImageId = imageId,
                Phrases = phrases,
                Score = score
            };

            await CloudTableService.Insert(imageComment, commentTable);
        }
    }
}