using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Services.Entities.JSON;
using Services.Entities.JSON.Handwriting;

namespace Functions.Functions
{
    public static class SubmitHandwriting
    {
        [FunctionName("SubmitHandwriting")]
        public static async Task Run(
            [BlobTrigger("images/{name}")] Stream trigger,
            [Queue("Handwriting")] ICollector<string> queueItem,
            string name, 
            TraceWriter log)
        {
            log.Info("Start");

            using (HttpContent content = new StreamContent(trigger))
            {
                var parameters = "recognizeText?handwriting=true";

                var response = await CognitiveServicesHttpClient.PostVisionRequest(content, parameters);

                if (response.IsSuccessStatusCode)
                {
                    var operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();

                    var request = new HandwritingRequest
                    {
                        Key               = name,
                        OperationLocation = operationLocation
                    };

                    var json = JSONHelper.ToJson(request);

                    queueItem.Add(json);
                }
            }

            log.Info("Finish");
        }
    }
}