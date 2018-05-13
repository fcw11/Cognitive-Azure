using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Funcs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Services.Implementation;

namespace Functions.Functions
{
    public static class Faces
    {
        [FunctionName("Faces")]
        public static async Task Run(
            [BlobTrigger("images/{name}")] Stream trigger,
            [Table("images")] CloudTable cloudTable,
            string name,
            TraceWriter log)
        {
            log.Info("Start");

            using (HttpContent content = new StreamContent(trigger))
            {
                var parameters = "detect" +
                                 "?returnFaceId=true" +
                                 "&returnFaceLandmarks=true" +
                                 "&returnFaceAttributes=age,gender,smile,facialHair,headPose,glasses,emotion,hair,makeup,accessories,blur,exposure,noise";

                var response = await CognitiveServicesHttpClient.PostFaceRequest(content, parameters);

                if (response.IsSuccessStatusCode)
                {
                    var responseBytes = await response.Content.ReadAsStringAsync();

                    await cloudTable.Update(name, responseBytes, (image, text) =>
                    {
                        image.Faces = text;
                    });
                }
            }

            log.Info("Finish");
        }
    }
}