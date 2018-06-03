using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
            [TimerTrigger("0 0 22 * * *")]TimerInfo myTimer, 
            [Table("Images")] CloudTable imageTable,
            [Table("ImageComments")] CloudTable imageCommentTable,
            [Table("AudioIdentificationProfiles")] CloudTable audioIdentificationProfilesTable,
            [Table("AudioVerificationProfiles")] CloudTable audioVerficationProfilesTable,
            [Blob("images", FileAccess.Read)] CloudBlobContainer imagesContainer,
            [Blob("thumbnails", FileAccess.Read)] CloudBlobContainer thumbnailsContainer,
            [Blob("faces", FileAccess.Read)] CloudBlobContainer facesContainer,
            TraceWriter log)
        {
            var imageTableEntities = imageTable.ExecuteQuery(new TableQuery()).ToList();

            foreach (var entity in imageTableEntities)
            {
                await imageTable.ExecuteAsync(TableOperation.Delete(entity));
            }

            var imageCommentTableEntities = imageCommentTable.ExecuteQuery(new TableQuery()).ToList();

            foreach (var entity in imageCommentTableEntities)
            {
                await imageCommentTable.ExecuteAsync(TableOperation.Delete(entity));
            }

            var audioIdentificationProfileTableEntities = audioIdentificationProfilesTable.ExecuteQuery(new TableQuery()).ToList();

            foreach (var entity in audioIdentificationProfileTableEntities)
            {
                await audioIdentificationProfilesTable.ExecuteAsync(TableOperation.Delete(entity));
            }

            var audioVerificationProfileTableEntities = audioVerficationProfilesTable.ExecuteQuery(new TableQuery()).ToList();

            foreach (var entity in audioVerificationProfileTableEntities)
            {
                await audioVerficationProfilesTable.ExecuteAsync(TableOperation.Delete(entity));
            }

            var imageBlobEntities = imagesContainer.ListBlobs(string.Empty, true);

            foreach (CloudBlockBlob entity in imageBlobEntities)
            {
                await entity.DeleteAsync();
            }

            var thumbnailBlobEntities = thumbnailsContainer.ListBlobs(string.Empty, true);

            foreach (CloudBlockBlob entity in thumbnailBlobEntities)
            {
                await entity.DeleteAsync();
            }

            var faceBlobEntities = facesContainer.ListBlobs(string.Empty, true);

            foreach (CloudBlockBlob entity in faceBlobEntities)
            {
                await entity.DeleteAsync();
            }

            using (var client = new HttpClient())
            {
                var faceVerificationDelete = ConfigurationManager.AppSettings["FaceVerificationDelete"];
                await client.GetAsync(faceVerificationDelete);

                var audioIdentificationDelete = ConfigurationManager.AppSettings["AudioIdentificationDelete"];
                await client.GetAsync(audioIdentificationDelete);

                var audioVerificatonDelete = ConfigurationManager.AppSettings["AudioVerificatonDelete"];
                await client.GetAsync(audioVerificatonDelete);
            }
        }
    }
}