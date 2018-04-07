using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;

namespace Funcs.Functions
{
    public static class TagImage
    {
        [FunctionName("TagImage")]
        public static async Task Run(
            [BlobTrigger("images/{name}")] Stream trigger,
            [Table("images")] CloudTable cloudTable,
            string name, 
            TraceWriter log)
        {
            log.Info("Start");

            using (HttpContent content = new StreamContent(trigger))
            {
                var parameters = "tag";

                var response = await CognitiveServicesHttpClient.PostRequest(content, parameters);

                if (response.IsSuccessStatusCode)
                {
                    var responseBytes = await response.Content.ReadAsStringAsync();

                    await cloudTable.Update(name, responseBytes, (image, text) =>
                    {
                        image.Tag = text;
                    });
                }
            }

            log.Info("Finish");
        }
    }
}