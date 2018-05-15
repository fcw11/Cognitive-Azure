using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Services.Entities;
using Services.Interfaces;

namespace Services.Implementation
{
    public class CloudTableService : ICloudTableService
    {
        public IConfiguration Configuration { get; set; }

        private CloudTableClient CloudTableClient { get; }

        public CloudTableService(IConfiguration configuration)
        {
            Configuration = configuration;

            var cloudConnectionString = Configuration["BlobConnectionString"];

            var cloudStorageAccount = CloudStorageAccount.Parse(cloudConnectionString);

            CloudTableClient = cloudStorageAccount.CreateCloudTableClient();
        }

        public async Task CreateTablesIfNotExist()
        {
            var imagesContainerName = Configuration["ImageContainerName"];

            var cloudTable = CloudTableClient.GetTableReference(imagesContainerName);

            await cloudTable.CreateIfNotExistsAsync();

            var commentTable = Configuration["CommentsTable"];

            var imageCommentsTable = CloudTableClient.GetTableReference(commentTable);

            await imageCommentsTable.CreateIfNotExistsAsync();
        }

        public async Task AddComment(Guid imageId, string comment, double score)
        {
            var commentTable = Configuration["CommentsTable"];

            var table = CloudTableClient.GetTableReference(commentTable);

            var imageComment = new ImageComment
            {
                PartitionKey = imageId.ToString(),
                Comment = comment,
                ImageId = imageId,
                Score = score
            };

            var insert = TableOperation.Insert(imageComment);

            await table.ExecuteAsync(insert);
        }

        public async Task<IQueryable<ImageComment>> RetrieveComments(Guid imageId)
        {
            var commentTable = Configuration["CommentsTable"];

            var table = CloudTableClient.GetTableReference(commentTable);

            var query = new TableQuery<ImageComment>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, imageId.ToString()));

            var result = await table.ExecuteQuerySegmentedAsync(query, null);

            return result.Results.AsQueryable();
        }

        public async Task UploadImageInformationToTable(IFormFile file, Guid imageId, Uri blobUri)
        {
            var imagesContainerName = Configuration["ImageContainerName"];

            var table = CloudTableClient.GetTableReference(imagesContainerName);

            var fileName = Path.GetFileName(file.FileName);
            var entity = new Image(imageId) { Name = fileName, Uri = blobUri.AbsoluteUri };
            var insertOperation = TableOperation.Insert(entity);

            await table.ExecuteAsync(insertOperation);
        }

        public IQueryable<Image> RetrieveImages()
        {
            var imagesContainerName = Configuration["ImageContainerName"];

            var table = CloudTableClient.GetTableReference(imagesContainerName);

            var tableQuery = new TableQuery<Image>();
            var tableQueryResult = table.ExecuteQuerySegmentedAsync(tableQuery, null).Result;

            return tableQueryResult.Results.AsQueryable();
        }

        public Image RetrieveImage(Guid id)
        {
            var imagesContainerName = Configuration["ImageContainerName"];

            var table = CloudTableClient.GetTableReference(imagesContainerName);

            var tableQuery = new TableQuery<Image>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString()));
            var tableQueryResult = table.ExecuteQuerySegmentedAsync(tableQuery, null).Result;

            return tableQueryResult.Results.Single();
        }
    }
}