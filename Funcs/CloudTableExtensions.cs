using System.Threading.Tasks;
using Funcs.Model;
using Microsoft.WindowsAzure.Storage.Table;

namespace Funcs
{
    public static class CloudTableExtensions
    {

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
    }
}
