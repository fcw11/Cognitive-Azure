using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Funcs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Functions.Functions
{
    public static class CreateThumbnail
    {
        [FunctionName("CreateThumbnail")]
        public static async Task Run(
            [BlobTrigger("images/{name}")] Stream trigger, 
            [Blob("thumbnails/{name}", FileAccess.ReadWrite)] ICloudBlob thumbnail,
            [Table("images")] CloudTable cloudTable,
            string name,
            TraceWriter log
        )
        {
            log.Info("Start");

            using (HttpContent content = new StreamContent(trigger))
            {
                var parameters = "generateThumbnail?width=200&height=150&smartCropping=true";

                var response = await CognitiveServicesHttpClient.PostRequest(content, parameters);

                if (response.IsSuccessStatusCode)
                {
                    var responseBytes = await response.Content.ReadAsStreamAsync(); 

                    await thumbnail.UploadFromStreamAsync(responseBytes);

                    await cloudTable.Update(name, thumbnail.Uri.AbsoluteUri, (image, text) =>
                    {
                        image.ThumbUri = text;
                    });
                }
            }

            log.Info("Finish");
        }
    }
}