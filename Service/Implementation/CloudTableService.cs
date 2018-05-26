using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
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
            var list = new List<string>
            {
                "ImagesTable",
                "CommentsTable",
                "AudioProfileTable"
            };

            foreach (var configurationName in list)
            {
                var containerName = Configuration[configurationName];

                var table = CloudTableClient.GetTableReference(containerName);

                await table.CreateIfNotExistsAsync();
            }
        }

        public async Task<TableResult> Insert<T>(T entity, string tableName) where T : ITableEntity
        {
            var table = CloudTableClient.GetTableReference(tableName);

            var insertOperation = TableOperation.Insert(entity);

            try
            {
                return await table.ExecuteAsync(insertOperation);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IQueryable<T>> Retrieve<T>(string tableName) where T : ITableEntity, new()
        {
            var table = CloudTableClient.GetTableReference(tableName);

            var query = new TableQuery<T>();

            var tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            return tableQueryResult.Results.AsQueryable();
        }

        public async Task<IQueryable<T>> Retrieve<T>(Guid pertitionKey, string tableName) where T : ITableEntity, new()
        {
            var table = CloudTableClient.GetTableReference(tableName);

            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, pertitionKey.ToString()));

            var result = await table.ExecuteQuerySegmentedAsync(query, null);

            return result.Results.AsQueryable();
        }

        public async Task<T> RetrieveSingle<T>(Guid rowKey, string tableName) where T : ITableEntity, new()
        {
            var table = CloudTableClient.GetTableReference(tableName);

            var tableQuery = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey.ToString()));

            var tableQueryResult = await table.ExecuteQuerySegmentedAsync(tableQuery, null);

            return tableQueryResult.Results.Single();
        }
    }
}