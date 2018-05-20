using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Services.Interfaces
{
    public interface ICloudTableService
    {
        Task CreateTablesIfNotExist();

        Task<TableResult> Insert<T>(T entity, string tableName) where T : ITableEntity;

        Task<IQueryable<T>> Retrieve<T>(string tableName) where T : ITableEntity, new();

        Task<IQueryable<T>> Retrieve<T>(Guid pertitionKey, string tableName) where T : ITableEntity, new();

        Task<T> RetrieveSingle<T>(Guid rowKey, string tableName) where T : ITableEntity, new();
    }
}