using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Funcs.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Funcs.Functions
{
    public static class CreateThumbnail
    {
        [FunctionName("CreateThumbnail")]
        public static async Task Run(
            [BlobTrigger("images/{name}")] Stream trigger, 
            [Blob("thumbnails/{name}", FileAccess.ReadWrite)] ICloudBlob thumbnail,
            [Table("images", "{name}", "{name}", Take = 1)] Image imageDetails,
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

                    imageDetails.ThumbUri = thumbnail.Uri.AbsoluteUri;
                }
            }

            log.Info("Finish");
        }
    }
}