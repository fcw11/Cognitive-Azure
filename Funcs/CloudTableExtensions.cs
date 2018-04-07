using System;
using System.Threading;
using System.Threading.Tasks;
using Funcs.Model;
using Microsoft.WindowsAzure.Storage.Table;

namespace Funcs
{
    public static class CloudTableExtensions
    {
        public delegate void Del(Image message, string responseBytes);

        public static async Task<Image> Retrieve(this CloudTable cloudTable, string name)
        {
            var retrieve = TableOperation.Retrieve<Image>(name, name);

            var tableResult = await cloudTable.ExecuteAsync(retrieve);

            return tableResult.Result as Image;
        }

        public static async Task Merge(this CloudTable cloudTable, Image image)
        {
            var mergeOperation = TableOperation.Merge(image);
            
            await cloudTable.ExecuteAsync(mergeOperation);
        }

        public static async Task Update(this CloudTable cloudTable, string rowKey, string responseBytes, Del updateItem, int count = 0)
        {
            try
            {
                var retrieve = TableOperation.Retrieve<Image>(rowKey, rowKey);

                var tableResult = await cloudTable.ExecuteAsync(retrieve);

                var existingImage = tableResult.Result as Image;

                updateItem.Invoke(existingImage, responseBytes);

                var mergeOperation = TableOperation.Merge(existingImage);

                await cloudTable.ExecuteAsync(mergeOperation);
            }
            catch (Exception ex)
            {
                if (count < 5)
                {
                    var r = new Random().Next(0, 5);

                    Thread.Sleep(r * 1000);

                    await Update(cloudTable, rowKey, responseBytes, updateItem, ++count);
                }
                else
                {
                    throw ex;
                }
            }
        }
    }
}
