using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Funcs.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Functions.Functions
{
    public static class CleanDown
    {
        [FunctionName("CleanDown")]
        public static async Task Run(
            [TimerTrigger("0 0 1 * * *")]TimerInfo myTimer, 
            [Table("images")] CloudTable cloudTable, 
            [Blob("images", FileAccess.Read)] CloudBlobContainer imagesContainer,
            [Blob("thumbnails", FileAccess.Read)] CloudBlobContainer thumbnailsContainer,
            [Blob("faces", FileAccess.Read)] CloudBlobContainer facesContainer,
            TraceWriter log)
        {
            var entities = cloudTable.ExecuteQuery(new TableQuery<Image>()).ToList();

            foreach (var entity in entities)
            {
                await cloudTable.ExecuteAsync(TableOperation.Delete(entity));
            }

            var images = imagesContainer.ListBlobs(string.Empty, true);

            foreach (CloudBlockBlob item in images)
            {
                await item.DeleteAsync();
            }

            var thumbnails = thumbnailsContainer.ListBlobs(string.Empty, true);

            foreach (CloudBlockBlob item in thumbnails)
            {
                await item.DeleteAsync();
            }

            var faces = facesContainer.ListBlobs(string.Empty, true);

            foreach (CloudBlockBlob item in faces)
            {
                await item.DeleteAsync();
            }
        }
    }
}